Imports VB = Microsoft.VisualBasic
Imports System.Data
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Data.SqlClient
Imports EgsReport.clsGlobal
Imports System.Windows.Forms
Imports EgsData
Imports System.IO
Imports log4net
Imports System.Globalization
Imports System.ComponentModel

Public Class xrReports
    Inherits DevExpress.XtraReports.UI.XtraReport
    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

#Region " Declarations "
    Dim intFieldLength(2) As Double
    Dim dblPertFieldWidth(2) As Double
    Dim strColumnName(2) As String
    Dim ColumnName As String
    Dim G_smallFontSize As Single
    Dim strWhereClause As String
    Dim fntFont As Font
    Dim fntFontHeader As Font

    Dim dblNutrientFactor As Double
    'Private WithEvents PageHeader As DevExpress.XtraReports.UI.PageHeaderBand

    Dim L_Nutrient(0 To 16) As clsGlobal.tNutrient
    Friend WithEvents XrLabel1 As DevExpress.XtraReports.UI.XRLabel
    Public G_intMaxRecipeDetailsWidth As Integer
    Dim L_lngCol(0 To 15) As Long
    Dim L_lngNameW As Long
    Dim L_lngName As Long
    Dim L_lngSubName As Long

    '30.11.2005
    Dim intAvailableWidth As Integer
    Dim intCurrentX As Integer
    Dim intCurrentY As Integer
    Dim strReportTitle As String
    Dim intTextHeight As Integer
    Dim intColumnSpace As Integer = 5
    Dim sf As StringFormat = New StringFormat(StringFormatFlags.NoClip)
    Dim fntReportTitle As Font
    Dim fntRegular As Font
    Dim fntBold As Font
    Dim fntItalic As Font
    Dim fntRegular2 As Font
    Dim fntFooter As Font
    Private WithEvents xrLblEGS As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents xrPIPageNumber As DevExpress.XtraReports.UI.XRPageInfo
    Private WithEvents xrLinePF As DevExpress.XtraReports.UI.XRLine
    Private WithEvents xrLblEGS2 As DevExpress.XtraReports.UI.XRLabel 'VRP 06.08.2008
    Public WithEvents xrRecipeNo As DevExpress.XtraReports.UI.XRLabel

    Dim fntBold2 As Font

    Dim strX As String
    Private WithEvents lblID As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents lblID2 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents lblID3 As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents SubRptDetail As DevExpress.XtraReports.UI.Subreport
    Dim L_NutrientCount As Integer
    Private WithEvents lblMain As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents lblParent As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents lblprintdetailsID As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrFooterPicLogo As DevExpress.XtraReports.UI.XRPictureBox

    Private WithEvents lbldescription As DevExpress.XtraReports.UI.XRLabel 'LD20160517
    Private WithEvents lblcopyright As DevExpress.XtraReports.UI.XRLabel 'LD20160517
    Private WithEvents xrLineReport As DevExpress.XtraReports.UI.XRLine 'LD20160517

    Dim i As Integer

    Dim m_flagSubRecipeAsterisk As Boolean = False 'DLS 08.09.2007
    Dim m_flagSubRecipeNormalFont As Boolean = False 'DLS 08.09.2007
    Dim m_strFooterAddress As String = "" 'DLS 28.08.2007
    Dim m_strFooterLogoPath As String = "" 'DLS 28.08.2007
    Dim m_flagOnePictureRight As Boolean = False  'DLS 28.08.2007
    Dim m_flagNoLines As Boolean = False  'DLS 28.08.2007
    Dim m_strTitleColor As String = "" 'DLS 
    Dim m_flagRecipeDetail As Integer = -1 'VRP 14.12.2007
    Dim m_strMigrosParam As String = ";;;;" 'VRP 14.12.2007
    Dim m_blnThumbnailsView As String = False 'VRP 17.03.2008
    Dim m_dtProcedureTemplate As DataTable = Nothing 'VRP 16.04.2008
    Dim m_strCnn As String = Nothing
    Dim m_udtUser As structUser 'VRP 30.07.2008
    Dim m_strSelectedWeek As String = "" 'VRP 30.07.2008
    Dim m_dblLeftMargin As Integer 'VRP 29.08.2008 for footer recipe center
    Dim m_strSiteUrl As String = "" 'VRP 29.08.2008 for footer recipe center
    Dim m_MPPrintStyle As enumMPStyle  'VRP 24.10.2008
    Dim m_nCodeUserPlan As String 'VRP 30.04.2009

    'NEW FONTS 'VRP 25.06.2009
    Dim fntHeader1 As Font
    Dim fntHeader2 As Font
    Dim fntDetail1 As Font
    Dim fntDetail2 As Font
    Dim strUserLocale As String
#End Region

#Region " Designer generated code "

    Public Sub New(ByVal intCodeLang As Integer, ByVal strCnn As String)
        MyBase.New()

        'This call is required by the Designer.
        InitializeComponent()

        'Dim clsSysPref As clsSystem = New clsSystem(enumAppType.WebApp, strCnn)
        'clsSysPref.FetchReturnType = enumEgswFetchType.DataTable
        'Dim rwAccount As DataRow = clsSysPref.GetSystem.Rows(0)
        'If Not IsDBNull(rwAccount("PrgKey")) AndAlso rwAccount("PrgKey").ToString.Trim = "student" Then
        '    Dim textWatermark As Drawing.Watermark = New Drawing.Watermark()

        '    Dim cLang As clsEGSLanguage = New clsEGSLanguage(intCodeLang)
        '    textWatermark.Text = cLang.GetString(clsEGSLanguage.CodeType.StudentVersion)
        '    textWatermark.TextDirection = Drawing.DirectionMode.ForwardDiagonal
        '    textWatermark.Font = New Font(textWatermark.Font.FontFamily, 40)
        '    textWatermark.ForeColor = Color.DodgerBlue
        '    textWatermark.TextTransparency = 150
        '    textWatermark.ShowBehind = True
        '    'textWatermark.PageRange = "1,3-5"
        '    Me.Watermark.CopyFrom(textWatermark)
        'End If
        'Add any initialization after the InitializeComponent() call     

    End Sub

    'XtraReport overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Designer
    'It can be modified using the Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand
        Me.lblMain = New DevExpress.XtraReports.UI.XRLabel
        Me.lblParent = New DevExpress.XtraReports.UI.XRLabel
        Me.lblprintdetailsID = New DevExpress.XtraReports.UI.XRLabel
        Me.lblID = New DevExpress.XtraReports.UI.XRLabel
        Me.lblID2 = New DevExpress.XtraReports.UI.XRLabel
        Me.lblID3 = New DevExpress.XtraReports.UI.XRLabel
        Me.SubRptDetail = New DevExpress.XtraReports.UI.Subreport
        Me.PageHeader = New DevExpress.XtraReports.UI.PageHeaderBand
        Me.PageFooter = New DevExpress.XtraReports.UI.PageFooterBand
        Me.XrFooterPicLogo = New DevExpress.XtraReports.UI.XRPictureBox
        Me.xrLinePF = New DevExpress.XtraReports.UI.XRLine
        Me.xrPIPageNumber = New DevExpress.XtraReports.UI.XRPageInfo
        Me.xrLblEGS = New DevExpress.XtraReports.UI.XRLabel
        Me.xrLblEGS2 = New DevExpress.XtraReports.UI.XRLabel
        Me.lbldescription = New DevExpress.XtraReports.UI.XRLabel 'LD20160517
        Me.lblcopyright = New DevExpress.XtraReports.UI.XRLabel 'LD20160517
        Me.xrLineReport = New DevExpress.XtraReports.UI.XRLine  'LD20160517
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.lblMain, Me.lblParent, Me.lblprintdetailsID, Me.lblID, Me.lblID2, Me.lblID3, Me.SubRptDetail})
        Me.Detail.Height = 55
        Me.Detail.Name = "Detail"
        Me.Detail.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand
        '
        'lblMain
        '
        Me.lblMain.ForeColor = System.Drawing.Color.White
        Me.lblMain.Location = New System.Drawing.Point(133, 0)
        Me.lblMain.Name = "lblMain"
        Me.lblMain.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.lblMain.ParentStyleUsing.UseForeColor = False
        Me.lblMain.Size = New System.Drawing.Size(50, 17)
        Me.lblMain.Text = "lblID"
        Me.lblMain.Visible = False
        '

        'LD20160517
        Me.lblcopyright.ForeColor = System.Drawing.Color.Black
        Me.lblcopyright.Font = New Font("Arial", 9, FontStyle.Regular)
        Me.lblcopyright.Location = New System.Drawing.Point(0, 9)
        Me.lblcopyright.Name = "lblCopyRight"
        Me.lblcopyright.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 20, 0, 0.0!)
        Me.lblcopyright.ParentStyleUsing.UseForeColor = False
        Me.lblcopyright.Size = New System.Drawing.Size(650, 10)
        Me.lblcopyright.Text = "Content is copyright c Vita-Mix Corporation. All rights reserved."
        Me.lblcopyright.Visible = False



        Me.xrLineReport.Location = New System.Drawing.Point(0, 0)
        Me.xrLineReport.Name = "xrLineReport"
        Me.xrLineReport.Size = New System.Drawing.Size(650, 9)
        Me.xrLineReport.Visible = False


        Me.lblcopyright.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        Me.PageFooter.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {lblcopyright})
        Me.PageFooter.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.xrLineReport})

        'lblParent
        '
        Me.lblParent.ForeColor = System.Drawing.Color.White
        Me.lblParent.Location = New System.Drawing.Point(75, 0)
        Me.lblParent.Name = "lblParent"
        Me.lblParent.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.lblParent.ParentStyleUsing.UseForeColor = False
        Me.lblParent.Size = New System.Drawing.Size(50, 17)
        Me.lblParent.Text = "lblID"
        Me.lblParent.Visible = False

        'lblprintdetailsid
        '
        Me.lblprintdetailsID.ForeColor = System.Drawing.Color.White
        Me.lblprintdetailsID.Location = New System.Drawing.Point(75, 0)
        Me.lblprintdetailsID.Name = "lblprintdetailsID"
        Me.lblprintdetailsID.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.lblprintdetailsID.ParentStyleUsing.UseForeColor = False
        Me.lblprintdetailsID.Size = New System.Drawing.Size(50, 17)
        Me.lblprintdetailsID.Text = "lblID"
        Me.lblprintdetailsID.Visible = False

        '
        'lblID
        '
        Me.lblID.ForeColor = System.Drawing.Color.White
        Me.lblID.Location = New System.Drawing.Point(17, 0)
        Me.lblID.Name = "lblID"
        Me.lblID.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.lblID.ParentStyleUsing.UseForeColor = False
        Me.lblID.Size = New System.Drawing.Size(50, 17)
        Me.lblID.Text = "lblID"
        Me.lblID.Visible = False
        '
        'SubRptDetail
        '
        Me.SubRptDetail.Location = New System.Drawing.Point(333, 17)
        Me.SubRptDetail.Name = "SubRptDetail"
        Me.SubRptDetail.Visible = False
        '
        'PageHeader
        '
        Me.PageHeader.Height = 30
        Me.PageHeader.Name = "PageHeader"
        '
        'PageFooter
        '
        Me.PageFooter.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrFooterPicLogo, Me.xrLinePF, Me.xrPIPageNumber, Me.xrLblEGS, Me.xrLblEGS2})
        Me.PageFooter.Height = 30
        Me.PageFooter.Name = "PageFooter"
        '
        'XrFooterPicLogo
        '
        Me.XrFooterPicLogo.Location = New System.Drawing.Point(492, 8)
        Me.XrFooterPicLogo.Name = "XrFooterPicLogo"
        Me.XrFooterPicLogo.Size = New System.Drawing.Size(157, 17)
        '
        'xrLinePF
        '
        Me.xrLinePF.Location = New System.Drawing.Point(0, 0)
        Me.xrLinePF.Name = "xrLinePF"
        Me.xrLinePF.Size = New System.Drawing.Size(650, 9)
        '
        'xrPIPageNumber
        '
        Me.xrPIPageNumber.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.xrPIPageNumber.Format = "Page {0} of {1}"
        Me.xrPIPageNumber.Location = New System.Drawing.Point(508, 11)
        Me.xrPIPageNumber.Name = "xrPIPageNumber"
        Me.xrPIPageNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrPIPageNumber.ParentStyleUsing.UseFont = False
        Me.xrPIPageNumber.Size = New System.Drawing.Size(142, 17)
        Me.xrPIPageNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'xrLblEGS
        '
        Me.xrLblEGS.Font = New System.Drawing.Font("Arial", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.xrLblEGS.KeepTogether = True
        Me.xrLblEGS.Location = New System.Drawing.Point(0, 8)
        Me.xrLblEGS.Multiline = True
        Me.xrLblEGS.Name = "xrLblEGS"
        Me.xrLblEGS.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLblEGS.ParentStyleUsing.UseFont = False
        Me.xrLblEGS.Size = New System.Drawing.Size(492, 17)
        '
        'xrLblEGS2
        '
        Me.xrLblEGS2.Font = New System.Drawing.Font("Arial", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.xrLblEGS2.KeepTogether = True
        Me.xrLblEGS2.Location = New System.Drawing.Point(0, 8)
        Me.xrLblEGS2.Multiline = True
        Me.xrLblEGS2.Name = "xrLblEGS2"
        Me.xrLblEGS2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.xrLblEGS2.ParentStyleUsing.UseFont = False
        Me.xrLblEGS2.Size = New System.Drawing.Size(492, 17)
        Me.xrLblEGS2.Visible = False
        '
        'xrReports
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.PageHeader, Me.PageFooter})
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents PageHeader As DevExpress.XtraReports.UI.PageHeaderBand
    Friend WithEvents PageFooter As DevExpress.XtraReports.UI.PageFooterBand

#End Region

#Region " Public Property "
    Public Property TitleColor() As String  'DLS 09.08.2007
        Get
            Return m_strTitleColor
        End Get
        Set(ByVal value As String)
            m_strTitleColor = value
        End Set
    End Property

    Public Property NoPrintLines() As Boolean   'DLS 09.08.2007
        Get
            Return m_flagNoLines
        End Get
        Set(ByVal value As Boolean)
            m_flagNoLines = value
        End Set
    End Property

    Public Property FooterAddress() As String  'DLS 09.08.2007
        Get
            Return m_strFooterAddress
        End Get
        Set(ByVal value As String)
            m_strFooterAddress = value
        End Set
    End Property

    Public Property FooterLogoPath() As String  'DLS 09.08.2007
        Get
            Return m_strFooterLogoPath
        End Get
        Set(ByVal value As String)
            m_strFooterLogoPath = value
        End Set
    End Property

    Public Property PictureOneRight() As Boolean   'DLS 09.08.2007
        Get
            Return m_flagOnePictureRight
        End Get
        Set(ByVal value As Boolean)
            m_flagOnePictureRight = value
        End Set
    End Property

    Public Property DisplaySubRecipeNormalFont() As Boolean 'DLS 09.08.2007
        Get
            Return m_flagSubRecipeNormalFont
        End Get
        Set(ByVal value As Boolean)
            m_flagSubRecipeNormalFont = value
        End Set
    End Property

    Public Property DisplaySubRecipeAstrisk() As Boolean 'DLS 09.08.2007
        Get
            Return m_flagSubRecipeAsterisk
        End Get
        Set(ByVal value As Boolean)
            m_flagSubRecipeAsterisk = value
        End Set
    End Property

    Public Property DisplayRecipeDetails() As Integer 'VRP 14.12.2007
        Get
            Return m_flagRecipeDetail
        End Get
        Set(ByVal value As Integer)
            m_flagRecipeDetail = value
        End Set
    End Property

    Public Property strMigrosParam() As String 'VRP 14.12.2007
        Get
            Return m_strMigrosParam
        End Get
        Set(ByVal value As String)
            m_strMigrosParam = value
        End Set
    End Property

    Public Property blnThumbnailsView() As Boolean 'VRP 17.03.2008
        Get
            Return m_blnThumbnailsView
        End Get
        Set(ByVal value As Boolean)
            m_blnThumbnailsView = value
        End Set
    End Property

    Public Property strCnn() As String 'VRP 16.04.2008
        Get
            Return m_strCnn
        End Get
        Set(ByVal value As String)
            m_strCnn = value
        End Set
    End Property

    Public Property udtUser() As structUser 'VRP 30.07.2008
        Get
            Return m_udtUser
        End Get
        Set(ByVal value As structUser)
            m_udtUser = value
        End Set
    End Property

    Public Property SelectedWeek() As String 'VRP 30.07.2008
        Get
            Return m_strSelectedWeek
        End Get
        Set(ByVal value As String)
            m_strSelectedWeek = value
        End Set
    End Property

    Public Property CodeUserPlan() As String 'VRP 30.04.2009
        Get
            Return m_nCodeUserPlan
        End Get
        Set(ByVal value As String)
            m_nCodeUserPlan = value
        End Set
    End Property

    Public Sub BindLabel(ByVal label As XRLabel, ByVal bindingMember As String)
        ' adding binding to the label's collection of bindings
        label.DataBindings.Add("Text", DataSource, bindingMember)
    End Sub

    Public Property SiteUrl() As String 'VRP 29.08.2008
        Get
            Return m_strSiteUrl
        End Get
        Set(ByVal value As String)
            m_strSiteUrl = value
        End Set
    End Property


    'Public Sub ChangeFont(ByVal label As XRLabel, ByVal fntFont As Font)
    '    ' adding binding to the label's collection of bindings
    '    label.Font = fntFont
    'End Sub

    'Public Sub BindPicture(ByVal pic As XRPictureBox, ByVal bindingMember As String)
    '    ' adding binding to the picturebox collection of bindings
    '    pic.DataBindings.Add("Image", DataSource, bindingMember)
    'End Sub

    Public Property MPPrintStyle() As enumMPStyle  'VRP 24.10.2008
        Get
            Return m_MPPrintStyle
        End Get
        Set(ByVal value As enumMPStyle)
            m_MPPrintStyle = value
        End Set
    End Property

#End Region

#Region " Function "
    '01 December 2005
    'Get Widest column space occupied
    Function fctGetWidest(ByVal strText As String, ByVal intTxtHt As Integer, ByVal intWidest As Integer, ByVal fntFont As Font) As Integer
        Dim intWidth As Integer
        With Me

            sf = New StringFormat(StringFormatFlags.DirectionVertical)

            intWidth = ReportingTextUtils.MeasureText(strText, fntFont, intTxtHt, sf, Me.Padding).Height
            If intWidest < intWidth Then 'FTB(1450)
                Return intWidth
            Else
                Return intWidest
            End If
        End With
    End Function

    Function fctGetHighest(ByVal strText As String, ByVal intTxtHt As Integer, ByVal intWidest As Integer, ByVal fntFont As Font, ByVal strFmt As StringFormat) As Integer
        Dim intHeight As Integer
        With Me
            intHeight = ReportingTextUtils.MeasureText(strText, fntFont, intWidest, strFmt, Me.Padding).Height
            If intTxtHt < intHeight Then 'FTB(1450)
                Return intHeight
            Else
                Return intTxtHt
            End If
        End With
    End Function

    Public Function fctResize(ByVal lbl As DevExpress.XtraReports.UI.XRLabel, ByVal fontsize As Single, ByVal fontname As String, ByVal font As Font, ByVal bUsedBold As Boolean) As Font
        Try
            fctResize = font
            'Resize Font if does not fit the column width
            Dim StrToMeasure As String
            ' Use the font used by the textbox
            Dim FontUsed As Font = font

            ' Create default graphics object
            Dim g As Graphics

            ' SizeF object to be returned by the MeasureString method
            Dim StrSize As New SizeF

            Dim b As Bitmap

BackHere:

            ' Compute the string dimensions in the given font
            b = New Bitmap(1, 1, PixelFormat.Format32bppArgb)
            g = Graphics.FromImage(b)

            '        g = fm.CreateGraphics
            StrToMeasure = lbl.Text
            ' Measure string
            If bUsedBold Then
                FontUsed = New Font(fontname, fontsize, FontStyle.Bold)
            Else
                FontUsed = New Font(fontname, fontsize, FontStyle.Regular)
            End If
            ' StrSize = g.MeasureString(StrToMeasure, FontUsed, lbl.Width)
            StrSize = g.MeasureString(StrToMeasure, FontUsed, lbl.Width)
            If StrSize.Height > 10 Then
                fontsize -= 1
                GoTo BackHere
            End If

            If fontsize < G_smallFontSize Then G_smallFontSize = fontsize
            Return FontUsed
            g.Dispose()
            b.Dispose()
        Catch ex As Exception
            Log.Info(ex.Message)
        End Try
    End Function

    Function GetLineSpace(ByVal intTextHeight As Integer) As Double
        Select Case G_ReportOptions.dblLineSpace
            Case 1
                Return intTextHeight
            Case Is < 1
                Return intTextHeight * G_ReportOptions.dblLineSpace
            Case Is > 1
                Return intTextHeight + ReportingTextUtils.MeasureText("A", fntRegular, 1, sf, Me.Padding).Height * (G_ReportOptions.dblLineSpace - 1)
        End Select
    End Function

    Function fctMakeLine(ByVal intX As Integer, ByVal intY As Integer, ByVal intWidth As Integer, ByVal intLineWidth As Integer, Optional ByVal intHeight As Integer = 9) As XRLine
        Dim XRLine As New XRLine

        With XRLine
            .Location = New System.Drawing.Point(intX, intY)
            .Name = "xrLine1"
            .Size = New System.Drawing.Size(intWidth, intHeight)
            .LineStyle = Drawing2D.DashStyle.Solid
            .LineWidth = intLineWidth
        End With
        Return XRLine
    End Function

    Public Function fctGetMaxHeight(ByVal strText As String, ByVal intWeight As Integer, ByVal fontsize As Single, ByVal fontname As String, ByVal font As Font, ByVal bUsedBold As Boolean) As Single
        Dim lb As New Label
        Dim StrToMeasure As String
        Dim FontUsed As Font = font

        Dim g As Graphics
        Dim StrSize As New SizeF
        Dim b As Bitmap
        Try
            b = New Bitmap(1, 1, PixelFormat.Format32bppArgb)
            g = Graphics.FromImage(b)
            StrToMeasure = strText

            If bUsedBold Then
                FontUsed = New Font(fontname, fontsize, FontStyle.Bold)
            Else
                FontUsed = New Font(fontname, fontsize, FontStyle.Regular)
            End If
            StrSize = g.MeasureString(StrToMeasure, FontUsed, intWeight)

            Return StrSize.Height
            g.Dispose()
            b.Dispose()
        Catch ex As Exception
            Log.Info(ex.Message)
        End Try
    End Function

    Function fctMakePictureBox(ByVal strFileName As String, ByVal intX As Integer, ByVal intY As Integer, ByVal intPicWidth As Integer, ByVal intPicHeight As Integer) As XRPictureBox
        Dim XRPic As New XRPictureBox
        Dim H1 As Long
        Dim W1 As Long
        Dim Aspect As Double
        Dim dblTempH As Double
        Dim dblTempW As Double

        fctMakePictureBox = Nothing
        With XRPic
            .Image = Image.FromFile(strFileName)
            .Location = New System.Drawing.Point(intX, intY)
            .Name = "XrPictureBox1"
            .Sizing = ImageSizeMode.ZoomImage
            .Size = New System.Drawing.Size(intPicWidth, intPicHeight)

            'mcm 01.24.05---------------------------------------
            H1 = intPicHeight
            W1 = intPicWidth

            Aspect = .Image.Height / .Image.Width

            If .Image.Height > .Image.Width Then
                dblTempH = CDbl(H1)
                dblTempW = dblTempH / Aspect
            Else
                dblTempW = CDbl(W1)
                dblTempH = dblTempW * Aspect
            End If

            Do While (dblTempW > CDbl(W1) Or dblTempH > CDbl(H1))
                dblTempW = (dblTempW * 0.999)
                dblTempH = (dblTempH * 0.999)
            Loop

            G_dblMaxWidthPicture = CLng(dblTempW)
            G_dblMaxHeightPicture = CLng(dblTempH)
            .Sizing = ImageSizeMode.StretchImage

            .Size = New System.Drawing.Size(G_dblMaxWidthPicture, G_dblMaxHeightPicture)
            '-------------------------------------
        End With
        Return XRPic



    End Function
#End Region

#Region " Print List "
    Function fctPrintMerchandisePriceList(ByVal dtMerchandiseList As DataTable, ByVal strSubHeading As String,
                                          ByVal intLanguage As Integer,
                                          ByVal blnIncludeNumber As Boolean, ByVal blnIncludeSupplier As Boolean,
                                          ByVal blnIncludeCategory As Boolean, ByVal blnIncludePrice1 As Boolean, ByVal blnIncludePrice2 As Boolean,
                                          ByVal strFontName As String, ByVal sgFontSize As Single,
                                          ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double,
                                          ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, Optional ByVal blLandscape As Boolean = False,
                                          Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16, Optional ByVal userLocale As String = "en-US") As XtraReport 'VRP 05.11.2007


        'If blnThumbnailsView = True Then 'VRP 17.03.2008
        '    Me.fctPrintMerchandiseThumbnailsList(dtMerchandiseList, strSubHeading, intLanguage, strFontName, _
        '      sgFontSize, dblPageWidth, dblPageHeight, dblLeftMargin, dblRightMargin, dblTopMargin, _
        '      dblBottomMargin, blLandscape, strFontTitleName, sgFontTitleSize)
        '    Return Me
        'End If


        Dim cLang As New clsEGSLanguage(intLanguage)
        Dim drvReport As DataRowView

        Dim blnOneCurrency As Boolean

        Dim strLastCurrency As String
        Dim strX As String
        Dim strSupplier As String
        Dim NumberWidth As Integer
        Dim SupplierWidth As Integer
        Dim CategoryWidth As Integer
        Dim Price1Width As Integer
        Dim Price2Width As Integer
        Dim strPriceFromat As String

        Cursor.Current = Cursors.WaitCursor

        sf = New StringFormat(StringFormatFlags.NoClip)

        Dim userCulture As CultureInfo = New CultureInfo(userLocale)

        Try
            'If G_ReportOptions.blnPictureOneRight Then
            '    fntReportTitle = New Font(strFontName, 14, FontStyle.Bold)
            'Else
            '    fntReportTitle = New Font(strFontName, 16, FontStyle.Bold)
            'End If
            fntReportTitle = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'VRP 05.11.2007

            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntBold = New Font(strFontName, sgFontSize, FontStyle.Bold)
            fntFooter = New Font(strFontName, 8, FontStyle.Bold)
        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try

        xrLblEGS.Font = fntFooter

        Try


            If dtMerchandiseList.DefaultView.Count > 0 Then
                With Me
                    'Me.DataMember = dtMerchandiseList.TableName.ToString
                    'Me.DataSource = dtMerchandiseList

                    '.XrLabel1.Visible = False

                    'Papersize
                    '------------

                    '.PaperKind = Printing.PaperKind.Custom
                    '.PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

                    'Margins
                    '------------
                    .Margins.Left = CInt(dblLeftMargin)
                    .Margins.Top = CInt(dblTopMargin)
                    .Margins.Bottom = CInt(dblBottomMargin)
                    .Margins.Right = CInt(dblRightMargin)

                    'Orientation
                    '-----------
                    If blLandscape Then
                        .Landscape = True
                        intAvailableWidth = .PageHeight
                    Else
                        .Landscape = False
                        intAvailableWidth = .PageWidth
                    End If

                    intAvailableWidth = intAvailableWidth - .Margins.Left - .Margins.Right - 10

                    .xrLinePF.Left = 0
                    .xrLinePF.Width = intAvailableWidth
                    .xrPIPageNumber.Left = intAvailableWidth / 2
                    .xrPIPageNumber.Width = intAvailableWidth / 2
                    .xrPIPageNumber.Font = fntBold

                    .xrLblEGS.Left = 0
                    .xrLblEGS.Width = intAvailableWidth / 2
                    .xrLblEGS.Font = fntBold
                    .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"


                    If NoPrintLines Then
                        .xrLinePF.Visible = False
                    End If
                    subReportFooter(intAvailableWidth, strFontName)

                    intCurrentX = 0
                    intCurrentY = 0

                    'Report Title
                    'AGL 2012.10.23 - CWM-1809 - from "Ingredient List" to "Merchandise List"
                    strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.ProductList) & "-" & cLang.GetString(clsEGSLanguage.CodeType.Price) 'Merchandise List
                    intTextHeight = ReportingTextUtils.MeasureText(strReportTitle, fntReportTitle, intAvailableWidth, sf, Me.Padding).Height
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strReportTitle, fntReportTitle, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})
                    intCurrentY += intTextHeight

                    'Sub Report Header
                    intTextHeight = ReportingTextUtils.MeasureText(strSubHeading, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strSubHeading, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

                    'Get Column Width of wach header
                    sf = New StringFormat(StringFormatFlags.DirectionVertical)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                    L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    NumberWidth = L_lngCol(1)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Products)
                    L_lngNameW = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Supplier)
                    L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    SupplierWidth = L_lngCol(3)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Category)
                    L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Price) & "-1"
                    L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    L_lngCol(6) = ReportingTextUtils.MeasureText("/", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    Price1Width = L_lngCol(5) + L_lngCol(6)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Price) & "-2"
                    L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    L_lngCol(8) = ReportingTextUtils.MeasureText("/", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    Price2Width = L_lngCol(7) + L_lngCol(8)
                    'L_lngCol(9) = ReportingTextUtils.MeasureText("WWW", fntBold, intTextHeight, sf, me.padding).Height + intColumnSpace

                    Dim strName As String = Nothing
                    Dim strNumber As String = Nothing

                    strLastCurrency = Nothing
                    For Each drvReport In dtMerchandiseList.DefaultView

                        strPriceFromat = drvReport("format").ToString()
                        'If Not blnIncludePrice1 And Not blnIncludePrice2 Then
                        '    If (UCase(strName) = UCase(drvReport("Name").ToString)) And UCase(strNumber) = UCase(drvReport("Number").ToString) Then GoTo GOHere
                        'End If

                        'If Not IsNothing(strLastCurrency) Then
                        '    If strLastCurrency <> drvReport("symbole").ToString Then
                        '        blnOneCurrency = False
                        '        strLastCurrency = drvReport("symbole").ToString
                        '    End If
                        'Else
                        '    blnOneCurrency = True
                        '    strLastCurrency = drvReport("symbole").ToString
                        'End If
                        If blnIncludeNumber Then
                            strX = drvReport("Number").ToString() 'Always force to typecase this to string even when value is null, GYG21
                            strX = Replace(strX, Chr(1), "")
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(1) Then
                                L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(1) = 0
                        End If

                        strX = drvReport("Name")
                        strX = Replace(strX, Chr(1), "")
                        If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngNameW Then
                            L_lngNameW = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        End If

                        If blnIncludeSupplier Then
                            strX = drvReport("suppliername")
                            strX = Replace(strX, Chr(1), "")
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(3) = 0
                        End If

                        If blnIncludeCategory Then
                            strX = drvReport("categoryname")
                            strX = Replace(strX, Chr(1), "")
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(4) Then
                                L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                'L_lngCol(4) = 95
                            End If
                        Else
                            L_lngCol(4) = 0
                        End If

                        If blnIncludePrice1 Then
                            If Not IsDBNull(drvReport("price1")) Then
                                'strX = Format(drvReport("price1"), strPriceFromat)
                                strX = Convert.ToDecimal(drvReport("price1")).ToString("N", userCulture)
                                If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                    L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                            End If

                            strX = "/" & drvReport("unitname1")
                            If Not IsDBNull(drvReport("unitname1")) Then
                                strX = "/" & Trim(drvReport("unitname1"))
                                If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                    L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                            End If

                            strX = drvReport("symbole")
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(5) = 0
                            L_lngCol(6) = 0
                        End If

                        If blnIncludePrice2 Then
                            If Not IsDBNull(drvReport("price2")) Then
                                'strX = Format(drvReport("price2"), strPriceFromat)
                                strX = Convert.ToDecimal(drvReport("price2")).ToString("N", userCulture)
                                If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                    L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                            End If

                            strX = "/" & drvReport("unitname2")
                            If Not IsDBNull(drvReport("unitname2")) Then
                                strX = "/" & Trim(drvReport("unitname2"))
                                If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                    L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                            End If

                            strX = drvReport("symbole")
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(7) = 0
                            L_lngCol(8) = 0
                        End If


                        strLastCurrency = drvReport("symbole")
                        'strNumber = drvReport("Number").ToString
                        'strName = drvReport("Name").ToString
GOHere:
                    Next

                    'If blnOneCurrency Then
                    '    L_lngCol(7) = 0
                    'Else
                    '    strLastCurrency = ""
                    'End If

                    'L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1))
                    ' L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(2))
                    'L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(3))
                    'L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(4))
                    'L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(5))
                    'L_lngNameW = Math.Abs(L_lngNameW - 100)
                    'L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(6))
                    'L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(7))
                    'L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(8))

                    NumberWidth = L_lngCol(1)
                    SupplierWidth = L_lngCol(3)
                    CategoryWidth = L_lngCol(4)
                    Price1Width = L_lngCol(5) + L_lngCol(6)
                    Price2Width = L_lngCol(7) + L_lngCol(8)

                    'If blnOneCurrency Then
                    '    L_lngCol(7) = 0
                    'Else
                    '    strLastCurrency = ""
                    'End If

                    L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(2))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(3))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(4))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(5))
                    'L_lngNameW = Math.Abs(L_lngNameW - 100)
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(6))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(7))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(8))

                    'If L_lngNameW < L_lngCol(3) Then
                    Dim newWidth = (L_lngNameW + SupplierWidth) / 2
                    If blnIncludeNumber Then
                        L_lngNameW = 90
                        L_lngCol(3) = 90
                    Else
                        L_lngNameW = 110
                        L_lngCol(3) = newWidth
                    End If

                    'If L_lngNameW < L_lngCol(4) Then
                    'Dim newWidth = ((L_lngNameW + SupplierWidth) * 3) / 2
                    'If blnIncludeSupplier Then L_lngCol(4) = 210 Else L_lngCol(4) = 330


                    sf = New StringFormat(StringFormatFlags.NoClip)
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntRegular, L_lngCol(1), sf, Me.Padding).Height

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Products)
                    intTextHeight = fctGetHighest(strX, intTextHeight, L_lngNameW, fntRegular, sf)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Supplier)
                    intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(3), fntRegular, sf)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Category)
                    intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(4), fntRegular, sf)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Price) & "-1"
                    intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(3), fntRegular, sf)

                    strX = cLang.GetString(clsEGSLanguage.CodeType.Price) & "-2"
                    intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(3), fntRegular, sf)

                    'Line
                    intCurrentY += (intTextHeight * 2)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})

                    'Column Headers
                    '--------------------------------------------------------------------------------------
                    'Number     Merchandise     Wastage     Price       Tax     Date
                    '--------------------------------------------------------------------------------------
                    intCurrentY += 10
                    If blnIncludeNumber Then
                        strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, NumberWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    End If

                    intCurrentX += NumberWidth
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Products)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngNameW, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

                    intCurrentX += L_lngNameW
                    If blnIncludeSupplier Then
                        strX = cLang.GetString(clsEGSLanguage.CodeType.Supplier)
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(3), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    End If

                    intCurrentX += L_lngCol(3)
                    If blnIncludeCategory Then
                        strX = cLang.GetString(clsEGSLanguage.CodeType.Category)
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(4), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                        intCurrentX += L_lngCol(4)
                    End If

                    If blnIncludePrice1 Then
                        Price1Width = L_lngCol(5) + L_lngCol(6)
                        strX = cLang.GetString(clsEGSLanguage.CodeType.Price) & "-1"
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(5), intTextHeight, IIf(blnIncludePrice2, DevExpress.XtraPrinting.TextAlignment.TopRight, DevExpress.XtraPrinting.TextAlignment.TopCenter))})
                        intCurrentY += intTextHeight

                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strLastCurrency, fntBold, intCurrentX, intCurrentY, L_lngCol(5), intTextHeight, IIf(blnIncludePrice2, DevExpress.XtraPrinting.TextAlignment.TopRight, DevExpress.XtraPrinting.TextAlignment.TopCenter))})
                        intCurrentY -= intTextHeight

                        'intCurrentX += NumberWidth + 10 '+ L_lngCol(1)
                        intCurrentX += Price1Width
                    End If

                    If blnIncludePrice2 Then
                        Price2Width = L_lngCol(7) + L_lngCol(8)
                        strX = cLang.GetString(clsEGSLanguage.CodeType.Price) & "-2"
                        '.PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, Price2Width - intColumnSpace, intTextHeight, IIf(blnIncludePrice1, DevExpress.XtraPrinting.TextAlignment.TopRight, DevExpress.XtraPrinting.TextAlignment.TopCenter))})
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(7), intTextHeight, IIf(blnIncludePrice1, DevExpress.XtraPrinting.TextAlignment.TopRight, DevExpress.XtraPrinting.TextAlignment.TopCenter))})
                        intCurrentY += intTextHeight

                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strLastCurrency, fntBold, intCurrentX, intCurrentY, L_lngCol(7), intTextHeight, IIf(blnIncludePrice1, DevExpress.XtraPrinting.TextAlignment.TopRight, DevExpress.XtraPrinting.TextAlignment.TopCenter))})
                        intCurrentY -= intTextHeight
                    End If

                    If blnIncludePrice1 Or blnIncludePrice2 Then
                        intCurrentY += intTextHeight
                    End If


                    .Detail.Controls.Clear()
                    intCurrentX = 0
                    'If blnIncludePrice Then
                    '    If blnOneCurrency Then
                    'intCurrentY += intTextHeight
                    '    End If
                    'End If
                    '-------------------------------------------------------------------------------------------------

                    intCurrentY += intTextHeight
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})

                    Dim intMaxHeight As Integer
                    Dim sgFontSizeTemp As Single
                    'Dim intTextHeightTemp As Integer
                    Dim strGroupHeader As String = Nothing
                    Dim blnNewGroup As Boolean
                    intCurrentY = 0
                    intMaxHeight = ReportingTextUtils.MeasureText("A", fntRegular, intAvailableWidth, sf, Me.Padding).Height

                    sf = New StringFormat(StringFormatFlags.MeasureTrailingSpaces)
                    strSupplier = Nothing
                    intCurrentY += 10
                    For Each drvReport In dtMerchandiseList.DefaultView
                        'If Not blnIncludePrice Then
                        '    If (UCase(strName) = UCase(drvReport("Name").ToString)) And (UCase(strNumber) = UCase(drvReport("Number").ToString)) Then GoTo GOHere2
                        'End If
                        intCurrentX = 0
                        fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                        sgFontSizeTemp = sgFontSize
                        blnNewGroup = False
                        'Per Category
                        'If G_ReportOptions.strSortBy = "CategoryName" Then
                        '    strX = CType(drvReport("CategoryName"), String)
                        '    If UCase(strGroupHeader) <> UCase(strX) Then
                        '        strGroupHeader = strX
                        '        blnNewGroup = True
                        '    End If
                        'ElseIf G_ReportOptions.strSortBy = "Supplier" Then
                        '    'Per Supplier
                        '    strX = CType(drvReport("suppliername"), String)
                        '    If UCase(strGroupHeader) <> UCase(strX) Then
                        '        strGroupHeader = strX
                        '        blnNewGroup = True
                        '    End If
                        '    ' ElseIf G_ReportOptions.strSortBy = "Price" And Not blnIncludePrice1 Then
                        '    'strX = "0"
                        '    'If Not IsDBNull(drvReport("realitemprice")) Then strX = Format(drvReport("realitemprice"), G_strPriceFormat)
                        '    'If Not IsDBNull(drvReport("UnitDisplayName")) Then strX &= "/" & Trim(drvReport("UnitDisplayName"))

                        '    'If UCase(strGroupHeader) <> UCase(strX) Then
                        '    '    strGroupHeader = strX
                        '    '    blnNewGroup = True
                        '    'End If
                        'ElseIf G_ReportOptions.strSortBy = "Number" And Not blnIncludeNumber Then
                        '    strX = drvReport("Number").ToString
                        '    If UCase(strGroupHeader) <> UCase(strX) Then
                        '        strGroupHeader = strX
                        '        blnNewGroup = True
                        '    End If
                        'End If

                        'If blnNewGroup Then
                        '    intCurrentY += 5
                        '    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroupHeader, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                        '    intCurrentY += intTextHeight + 5
                        'End If
                        'Number
                        If blnIncludeNumber Then
                            strX = drvReport("Number").ToString
                            strX = Replace(strX, Chr(1), "")
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, NumberWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                        End If

                        'Merchandise
                        intCurrentX += NumberWidth
                        strX = drvReport("Name").ToString
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})

                        'intTextHeightTemp = CInt(.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, me.padding).Height)
                        'intTextHeight = intTextHeightTemp
                        If G_ReportOptions.blShrinkToFit Then
                            '    Do While Not intMaxHeight >= CInt(.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, me.padding).Height)
                            '        sgFontSizeTemp -= 1
                            '        fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                            '    Loop
                            '    intTextHeight = intMaxHeight
                            '    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            'Else
                            '    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                        End If

                        intCurrentX += L_lngNameW
                        'supplier
                        If blnIncludeSupplier Then
                            If Not IsDBNull(drvReport("suppliername")) Then strX = drvReport("suppliername")
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(3), intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                        End If

                        'category
                        intCurrentX += L_lngCol(3)
                        If blnIncludeCategory Then
                            If Not IsDBNull(drvReport("categoryname")) Then strX = drvReport("categoryname")
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(4), intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                            intCurrentX += L_lngCol(4)
                        End If

                        'Price1
                        Dim exceedWidth1 As Boolean = False
                        Dim exceedWidth2 As Boolean = False
                        Dim defPriceWidth As Integer = 60
                        Dim defUnitWidth As Integer = 30
                        If Not blnIncludePrice1 Or Not blnIncludePrice2 Then
                            defPriceWidth *= 2
                            defUnitWidth *= 2
                        End If

                        If blnIncludePrice1 Then
                            strX = ""
                            'If Not IsDBNull(drvReport("price1")) Then strX = Format(drvReport("price1"), strPriceFromat)
                            If Not IsDBNull(drvReport("price1")) Then strX = Convert.ToDecimal(drvReport("price1")).ToString("N", userCulture)
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(5), intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True, , True)})

                            intCurrentX += L_lngCol(5)

                            If Not IsDBNull(drvReport("unitname1")) Then strX = "/" & drvReport("unitname1")
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6), intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})

                            intCurrentX += L_lngCol(6)

                            'strX = ""
                            'If Not IsDBNull(drvReport("price1")) Then strX = Format(drvReport("price1"), strPriceFromat)
                            'Dim widthPrice = ReportingTextUtils.MeasureText(strX, fntRegular, intMaxHeight, sf, Me.Padding).Height
                            'If widthPrice > defPriceWidth And Not IsDBNull(drvReport("price2")) Then
                            '    intCurrentX += 10
                            'End If
                            '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, NumberWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})

                            'If widthPrice > defPriceWidth And Not IsDBNull(drvReport("price2")) Then
                            '    intCurrentY += intTextHeight
                            '    exceedWidth1 = True
                            'Else
                            '    intCurrentX += NumberWidth
                            'End If

                            ''strX = "/" & drvReport("ItemUnit")
                            'If Not IsDBNull(drvReport("unitname1")) Then strX = "/" & Trim(drvReport("unitname1"))
                            'Dim widthUnit = ReportingTextUtils.MeasureText(strX, fntRegular, intMaxHeight, sf, Me.Padding).Height
                            'Dim defWidthUnit As Integer = NumberWidth - intColumnSpace
                            'If widthUnit > defUnitWidth Then
                            '    If Not exceedWidth1 Then
                            '        intCurrentY += intTextHeight
                            '        intCurrentX -= NumberWidth
                            '        exceedWidth1 = True
                            '    End If
                            '    defWidthUnit = widthUnit
                            'End If

                            '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, defWidthUnit, intMaxHeight, IIf(widthPrice > defPriceWidth And Not IsDBNull(drvReport("price2")), DevExpress.XtraPrinting.TextAlignment.TopRight, DevExpress.XtraPrinting.TextAlignment.TopLeft))})
                            'If exceedWidth1 Then
                            '    intCurrentY -= intTextHeight
                            'End If
                        End If

                        'If exceedWidth1 Then intCurrentX += NumberWidth
                        If blnIncludePrice2 Then
                            strX = ""
                            'If Not IsDBNull(drvReport("price2")) Then strX = Format(drvReport("price2"), strPriceFromat)
                            If Not IsDBNull(drvReport("price2")) Then strX = Convert.ToDecimal(drvReport("price2")).ToString("N", userCulture)
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(7), intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True, , True)})

                            intCurrentX += L_lngCol(7)

                            If Not IsDBNull(drvReport("unitname2")) Then strX = "/" & drvReport("unitname2")
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(8), intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})


                            'strX = ""
                            'If Not IsDBNull(drvReport("price2")) Then strX = Format(drvReport("price2"), strPriceFromat)
                            'Dim widthPrice2 = ReportingTextUtils.MeasureText(strX, fntRegular, intMaxHeight, sf, Me.Padding).Height
                            'If widthPrice2 > defPriceWidth Then
                            '    If Not exceedWidth1 Then
                            '        intCurrentX += 25
                            '    Else
                            '        intCurrentX += 20
                            '    End If
                            'End If
                            '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, NumberWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})

                            'If widthPrice2 > defPriceWidth Then
                            '    intCurrentY += intTextHeight
                            '    exceedWidth2 = True
                            'Else
                            '    intCurrentX += NumberWidth
                            'End If

                            ''strX = "/" & drvReport("ItemUnit")
                            'If Not IsDBNull(drvReport("unitname2")) Then strX = "/" & Trim(drvReport("unitname2"))
                            'Dim widthUnit2 = ReportingTextUtils.MeasureText(strX, fntRegular, intMaxHeight, sf, Me.Padding).Height
                            'Dim defWidthUnit2 As Integer = NumberWidth - (intColumnSpace * 2)
                            'If widthUnit2 > defUnitWidth Then
                            '    If Not exceedWidth2 Then
                            '        intCurrentY += intTextHeight
                            '        intCurrentX -= NumberWidth
                            '        exceedWidth2 = True
                            '        If Not exceedWidth1 Then
                            '            intCurrentX += 25
                            '        Else
                            '            intCurrentX += 20
                            '        End If
                            '    End If
                            '    defWidthUnit2 = widthUnit2
                            'End If
                            '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, defWidthUnit2, intMaxHeight, IIf(widthPrice2 > defPriceWidth, DevExpress.XtraPrinting.TextAlignment.TopRight, DevExpress.XtraPrinting.TextAlignment.TopLeft))})
                            'If exceedWidth2 Then
                            '    intCurrentY -= intTextHeight
                            'End If
                        End If

                        If exceedWidth1 Or exceedWidth2 Then
                            intCurrentY += intTextHeight
                        End If


                        'intCurrentX += L_lngCol(4) + intColumnSpace
                        'intCurrentY += intTextHeight 
                        intCurrentY += GetLineSpace(intTextHeight)
                        strNumber = drvReport("Number").ToString
                        strName = drvReport("Name").ToString
GoHere2:
                    Next

                    Cursor.Current = Cursors.Arrow
                End With
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Return Me
    End Function

    '01 December 2005
    Function fctPrintMerchandiseList(ByVal dtMerchandiseList As DataTable, ByVal strSubHeading As String,
              ByVal intLanguage As Integer,
              ByVal blnIncludeNumber As Boolean, ByVal blnIncludeWastage As Boolean,
              ByVal blnIncludeTax As Boolean, ByVal blnIncludeDate As Boolean, ByVal blnIncludePrice As Boolean,
              ByVal strFontName As String, ByVal sgFontSize As Single,
              ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double,
              ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, Optional ByVal blLandscape As Boolean = False,
              Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16,
                                     Optional ByVal userLocale As String = "en-US") As XtraReport 'VRP 05.11.2007


        If blnThumbnailsView = True Then 'VRP 17.03.2008
            Me.fctPrintMerchandiseThumbnailsList(dtMerchandiseList, strSubHeading, intLanguage, strFontName,
              sgFontSize, dblPageWidth, dblPageHeight, dblLeftMargin, dblRightMargin, dblTopMargin,
              dblBottomMargin, blLandscape, strFontTitleName, sgFontTitleSize)
            Return Me
        End If

        Dim userCulture As CultureInfo = New CultureInfo(userLocale)

        Dim cLang As New clsEGSLanguage(intLanguage)
        Dim drvReport As DataRowView

        Dim blnOneCurrency As Boolean

        Dim dblWastage As Double

        Dim strLastCurrency As String
        Dim strX As String
        Dim strSupplier As String

        Cursor.Current = Cursors.WaitCursor

        sf = New StringFormat(StringFormatFlags.NoClip)

        Try
            'If G_ReportOptions.blnPictureOneRight Then
            '    fntReportTitle = New Font(strFontName, 14, FontStyle.Bold)
            'Else
            '    fntReportTitle = New Font(strFontName, 16, FontStyle.Bold)
            'End If

            fntReportTitle = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'VRP 05.11.2007
            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntBold = New Font(strFontName, sgFontSize, FontStyle.Bold)
            fntFooter = New Font(strFontName, 8, FontStyle.Bold)
        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try
        xrLblEGS.Font = fntFooter

        'Try

        If dtMerchandiseList.DefaultView.Count > 0 Then
            With Me
                Me.DataMember = dtMerchandiseList.TableName.ToString
                Me.DataSource = dtMerchandiseList

                'AGL 2013.02.14 - CWP-260 clear controls before starting
                .Detail.Controls.Clear()

                '.XrLabel1.Visible = False

                'Papersize
                '------------
                .PaperKind = Printing.PaperKind.Custom
                .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

                'Margins
                '------------
                '.Margins.Left = CInt(G_ReportOptions.dblLeftMargin)
                '.Margins.Top = CInt(G_ReportOptions.dblTopMargin)
                '.Margins.Bottom = CInt(G_ReportOptions.dblBottomMargin)
                '.Margins.Right = CInt(G_ReportOptions.dblRightMargin)

                .Margins.Left = CInt(dblLeftMargin)
                .Margins.Top = CInt(dblTopMargin)
                .Margins.Bottom = CInt(dblBottomMargin)
                .Margins.Right = CInt(dblRightMargin)

                'Orientation
                '-----------
                If blLandscape Then
                    .Landscape = True
                    intAvailableWidth = .PageHeight
                Else
                    .Landscape = False
                    intAvailableWidth = .PageWidth
                End If

                intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin) - 10

                .xrLinePF.Left = 0
                .xrLinePF.Width = intAvailableWidth
                .xrPIPageNumber.Left = intAvailableWidth / 2
                .xrPIPageNumber.Width = intAvailableWidth / 2
                .xrPIPageNumber.Font = fntBold

                .xrLblEGS.Left = 0
                .xrLblEGS.Width = intAvailableWidth / 2
                .xrLblEGS.Font = fntBold
                .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

                If NoPrintLines Then
                    .xrLinePF.Visible = False
                End If
                subReportFooter(intAvailableWidth, strFontName)

                intCurrentX = 0
                intCurrentY = 0

                'Report Title
                'AGL 2012.10.23 - CWM-1808 - changed "Ingredient List" to "Merchandise List"
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.ProductList)   'Merchandise List
                intTextHeight = ReportingTextUtils.MeasureText(strReportTitle, fntReportTitle, intAvailableWidth, sf, Me.Padding).Height
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strReportTitle, fntReportTitle, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})
                intCurrentY += intTextHeight

                'Sub Report Header
                intTextHeight = ReportingTextUtils.MeasureText(strSubHeading, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strSubHeading, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})


                'Get Column Width of wach header
                sf = New StringFormat(StringFormatFlags.DirectionVertical)

                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                strX = cLang.GetString(clsEGSLanguage.CodeType.Wastage)
                L_lngCol(2) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                strX = cLang.GetString(clsEGSLanguage.CodeType.Price)
                L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                L_lngCol(4) = ReportingTextUtils.MeasureText("/", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
                L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                L_lngCol(7) = ReportingTextUtils.MeasureText("WWW", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace

                Dim strName As String = Nothing
                Dim strNumber As String = Nothing

                strLastCurrency = Nothing
                For Each drvReport In dtMerchandiseList.DefaultView
                    ' If Not blnIncludePrice Then
                    'If (UCase(strName) = UCase(drvReport("Name").ToString)) And UCase(strNumber) = UCase(drvReport("Number").ToString) Then GoTo GOHere
                    'End If

                    If Not IsNothing(strLastCurrency) Then
                        If strLastCurrency <> drvReport("Currency").ToString Then
                            blnOneCurrency = False
                            strLastCurrency = drvReport("Currency").ToString
                        End If
                    Else
                        blnOneCurrency = True
                        strLastCurrency = drvReport("Currency").ToString
                    End If


                    If blnIncludeNumber Then
                        If IsDBNull(drvReport("Number")) Then
                            strX = " "
                        Else
                            strX = drvReport("Number")
                        End If

                        strX = Replace(strX, Chr(1), "")
                        If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(1) Then
                            L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        End If
                    Else
                        L_lngCol(1) = 0
                    End If

                    If blnIncludeWastage Then
                        dblWastage = 0
                        dblWastage = drvReport("Totalwastage")
                        strX = Format(dblWastage, G_FormatWhole) & "%"
                        If strX = "%" Then strX = ""
                        If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(2) Then
                            L_lngCol(2) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        End If
                    Else
                        L_lngCol(2) = 0
                    End If


                    If blnIncludePrice Then
                        If Not IsDBNull(drvReport("realitemprice")) Then
                            strX = Format(drvReport("realitemprice"), G_strPriceFormat)
                            strX = Convert.ToDecimal(drvReport("realitemprice")).ToString("N", userCulture)
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        End If

                        strX = "/" & drvReport("ItemUnit")
                        If Not IsDBNull(drvReport("UnitDisplayName")) Then
                            strX = "/" & Trim(drvReport("UnitDisplayName"))
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(4) Then
                                L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        End If
                    Else
                        L_lngCol(3) = 0
                        L_lngCol(4) = 0
                    End If

                    If blnIncludeTax Then
                        If Not IsDBNull(drvReport("Tax")) Then
                            strX = Format(drvReport("Tax"), G_FormatOneDecimal) & "%"
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        End If
                    Else
                        L_lngCol(5) = 0
                    End If

                    If blnIncludeDate Then
                        If Not IsDBNull(drvReport("Dates")) Then
                            'strX = fctConvertDate(drvReport("Dates") & "")
                            'strX = DirectCast(drvReport("Dates"), DateTime).ToShortDateString 'JBQL format date automatically getting the culture info
                            strX = DirectCast(drvReport("Dates"), DateTime).ToString("d", userCulture) 'CMC_COOP-2165
                            If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        End If
                    Else
                        L_lngCol(6) = 0
                    End If

                    strX = drvReport("Currency")
                    If ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                        L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    End If

                    strNumber = drvReport("Number").ToString
                    strName = drvReport("Name").ToString
GOHere:
                Next

                If blnOneCurrency Then
                    L_lngCol(7) = 0
                Else
                    strLastCurrency = ""
                End If

                L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1))
                L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(2))
                L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(3))
                L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(4))
                L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(5))
                L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(6))
                L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(7))

                sf = New StringFormat(StringFormatFlags.NoClip)
                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                intTextHeight = ReportingTextUtils.MeasureText(strX, fntRegular, L_lngCol(1), sf, Me.Padding).Height

                strX = cLang.GetString(clsEGSLanguage.CodeType.Products)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngNameW, fntRegular, sf)

                strX = cLang.GetString(clsEGSLanguage.CodeType.Wastage)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(2), fntRegular, sf)

                strX = cLang.GetString(clsEGSLanguage.CodeType.Price)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(3), fntRegular, sf)

                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(5), fntRegular, sf)

                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(6), fntRegular, sf)

                'Line
                intCurrentY += (intTextHeight * 2)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})

                'Column Headers
                '--------------------------------------------------------------------------------------
                'Number     Merchandise     Wastage     Price       Tax     Date
                '--------------------------------------------------------------------------------------
                intCurrentY += 10
                If blnIncludeNumber Then
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                End If

                intCurrentX += L_lngCol(1)
                strX = cLang.GetString(clsEGSLanguage.CodeType.Products)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

                intCurrentX += L_lngNameW
                If blnIncludeWastage Then
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Wastage)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(2), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If

                intCurrentX += L_lngCol(2)
                If blnIncludePrice Then
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Price)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    intCurrentY += intTextHeight
                    If blnOneCurrency Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strLastCurrency, fntBold, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                        intCurrentY -= intTextHeight
                    End If
                End If

                intCurrentX += L_lngCol(3) + L_lngCol(4)
                If blnIncludeTax Then
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If

                intCurrentX += L_lngCol(5)
                If blnIncludeDate Then
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If

                intCurrentX = 0
                If blnIncludePrice Then
                    If blnOneCurrency Then
                        intCurrentY += intTextHeight
                    End If
                End If
                '-------------------------------------------------------------------------------------------------

                intCurrentY += intTextHeight
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})

                Dim intMaxHeight As Integer
                Dim sgFontSizeTemp As Single
                Dim intTextHeightTemp As Integer
                Dim strGroupHeader As String = Nothing
                Dim blnNewGroup As Boolean
                intCurrentY = 0
                intMaxHeight = ReportingTextUtils.MeasureText("A", fntRegular, intAvailableWidth, sf, Me.Padding).Height

                sf = New StringFormat(StringFormatFlags.MeasureTrailingSpaces)
                strSupplier = Nothing
                intCurrentY += 10
                For Each drvReport In dtMerchandiseList.DefaultView
                    'If Not blnIncludePrice Then
                    '    If (UCase(strName) = UCase(drvReport("Name").ToString)) And (UCase(strNumber) = UCase(drvReport("Number").ToString)) Then GoTo GOHere2
                    'End If
                    intCurrentX = 0
                    fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                    sgFontSizeTemp = sgFontSize
                    blnNewGroup = False

                    'Per Category
                    If G_ReportOptions.strSortBy = "CategoryName" Then
                        strX = CType(drvReport("CategoryName"), String)
                        If UCase(strGroupHeader) <> UCase(strX) Then
                            strGroupHeader = strX
                            blnNewGroup = True
                        End If
                    ElseIf G_ReportOptions.strSortBy = "Supplier" Then
                        'Per Supplier
                        strX = CType(drvReport("NameRef"), String)
                        If UCase(strGroupHeader) <> UCase(strX) Then
                            strGroupHeader = strX
                            blnNewGroup = True
                        End If

                    ElseIf G_ReportOptions.strSortBy = "Tax" And Not blnIncludeTax Then
                        'Per Tax
                        strX = Format(drvReport("Tax"), G_FormatOneDecimal) & "%"
                        If UCase(strGroupHeader) <> UCase(strX) Then
                            strGroupHeader = strX
                            blnNewGroup = True
                        End If
                    ElseIf G_ReportOptions.strSortBy = "Dates" And Not blnIncludeDate Then
                        If UCase(strGroupHeader) <> UCase(fctConvertDate(drvReport("Dates") & "")) Then
                            strGroupHeader = fctConvertDate(drvReport("Dates") & "")
                            blnNewGroup = True
                        End If
                    ElseIf G_ReportOptions.strSortBy = "Wastage" And Not blnIncludeWastage Then
                        dblWastage = 0
                        If Not IsDBNull(drvReport("Totalwastage")) Then dblWastage = drvReport("Totalwastage")
                        strX = Format(dblWastage, G_FormatWhole) & "%"

                        If UCase(strGroupHeader) <> UCase(strX) Then
                            strGroupHeader = strX
                            blnNewGroup = True
                        End If
                    ElseIf G_ReportOptions.strSortBy = "Price" And Not blnIncludePrice Then
                        strX = "0"
                        If Not IsDBNull(drvReport("realitemprice")) Then strX = Format(drvReport("realitemprice"), G_strPriceFormat)
                        If Not IsDBNull(drvReport("UnitDisplayName")) Then strX &= "/" & Trim(drvReport("UnitDisplayName"))

                        If UCase(strGroupHeader) <> UCase(strX) Then
                            strGroupHeader = strX
                            blnNewGroup = True
                        End If
                    ElseIf G_ReportOptions.strSortBy = "Number" And Not blnIncludeNumber Then
                        strX = drvReport("Number").ToString
                        If UCase(strGroupHeader) <> UCase(strX) Then
                            strGroupHeader = strX
                            blnNewGroup = True
                        End If
                    End If

                    If blnNewGroup Then
                        intCurrentY += 5
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroupHeader, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                        intCurrentY += intTextHeight + 5
                    End If

                    'Number
                    If blnIncludeNumber Then
                        strX = drvReport("Number").ToString
                        strX = Replace(strX, Chr(1), "")
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    End If

                    'Merchandise
                    intCurrentX += L_lngCol(1)
                    strX = drvReport("Name").ToString

                    intTextHeightTemp = CInt(ReportingTextUtils.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                    intTextHeight = intTextHeightTemp
                    If G_ReportOptions.blShrinkToFit Then
                        Do While Not intMaxHeight >= CInt(ReportingTextUtils.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                            sgFontSizeTemp -= 1
                            fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                        Loop
                        intTextHeight = intMaxHeight
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    Else
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                    End If


                    fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)


                    intCurrentX += L_lngNameW
                    'Waste
                    If blnIncludeWastage Then
                        dblWastage = 0
                        If Not IsDBNull(drvReport("Totalwastage")) Then dblWastage = drvReport("Totalwastage")
                        strX = Format(dblWastage, G_FormatWhole) & "%"
                        If strX = "%" Then strX = ""
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    End If

                    intCurrentX += L_lngCol(2)
                    'Price
                    Dim strPriceFormat As String
                    If blnIncludePrice Then
                        strPriceFormat = drvReport("curformat").ToString
                        strX = "0"
                        'If Not IsDBNull(drvReport("realitemprice")) Then strX = Format(drvReport("realitemprice"), strPriceFormat)
                        If Not IsDBNull(drvReport("realitemprice")) Then
                            ' Format realitemprice using the user's locale, without currency symbol
                            strX = Convert.ToDecimal(drvReport("realitemprice")).ToString("N", userCulture)
                        End If
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})

                        intCurrentX += L_lngCol(3) - intColumnSpace
                        'strX = "/" & drvReport("ItemUnit")
                        If Not IsDBNull(drvReport("UnitDisplayName")) Then strX = "/" & Trim(drvReport("UnitDisplayName"))
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    End If


                    intCurrentX += L_lngCol(4) + intColumnSpace
                    'Tax
                    If blnIncludeTax Then
                        If Not IsDBNull(drvReport("Tax")) Then
                            strX = Format(drvReport("Tax"), G_FormatOneDecimal) & "%"
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                        End If
                    End If

                    intCurrentX += L_lngCol(5) + intColumnSpace
                    'Dates
                    If blnIncludeDate Then
                        If Not IsDBNull(drvReport("Dates")) Then
                            'strX = fctConvertDate(drvReport("Dates") & "")
                            'strX = DirectCast(drvReport("Dates"), DateTime).ToShortDateString 'JBQL format date automatically getting the culture info
                            strX = DirectCast(drvReport("Dates"), DateTime).ToString("d", userCulture)  ' "d" is for short date format
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                        End If
                    End If

                    'intCurrentY += intTextHeight 
                    intCurrentY += GetLineSpace(intTextHeight)
                    strNumber = drvReport("Number").ToString
                    strName = drvReport("Name").ToString
GoHere2:

                Next

                Cursor.Current = Cursors.Arrow
                dtMerchandiseList.Reset() 'VRP 11.03.2008
            End With

        End If
        'Catch ex As Exception
        '    MsgBox(ex.Message)
        'End Try
        Return Me

    End Function

    '30 November 2005
    Function fctPrintShoppingList(ByVal dtShoppingList As DataTable, ByVal strSubHeading As String,
                                   ByVal blnWithGroup As Boolean, ByVal strGroupBy As String,
                                   ByVal blnIncludePrice As Boolean, ByVal blnIncludeGrossQty As Boolean, ByVal blnIncludeNetQty As Boolean,
                                   ByVal blnIncludeNumber As Boolean,
                                   ByVal strFontName As String, ByVal sgFontSize As Single,
                                   ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double,
                                   ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, Optional ByVal blLandscape As Boolean = False,
                                   Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16,
                                   Optional ByVal OneOrMetricImperial As Boolean = False, Optional ByVal OneOrMetricImperialQuantityBasis As Integer = 1,
                                   Optional ByVal blnIncludeImperialGrossQty As Boolean = False, Optional ByVal blnIncludeImperialNetQty As Boolean = False) As XtraReport 'VRP 15.11.2007

        Dim column As DataColumn
        Dim length As Integer = dtShoppingList.Columns.Count
        Dim clang As New clsEGSLanguage(G_ReportOptions.intPageLanguage)
        Dim drvReport As DataRowView

        Dim dblPrice As Double
        Dim dblQty As Double
        Dim dblMetQty As Double
        Dim dblImpQty As Double
        Dim dblUnitfactor As Double
        Dim dblMetUnitfactor As Double
        Dim dblImpUnitfactor As Double
        Dim dblGross As Double
        Dim dblMetGross As Double
        Dim dblImpGross As Double
        Dim dblfactor As Double
        Dim dblMetfactor As Double
        Dim dblImpfactor As Double
        Dim dblAmount As Double
        Dim dblMetImpAmount As Double

        Dim sgType As Single
        Dim strName As String
        Dim strNumber As String
        Dim strCurrency As String
        Dim strUnitFormat As String
        Dim strMetUnitFormat As String
        Dim strImpUnitFormat As String
        Dim strUnitName As String
        Dim strMetUnitName As String
        Dim strImpUnitName As String
        Dim strPriceUnit As String
        Dim strX As String
        Dim strGroup As String

        Dim strSupplier As String = Nothing
        Dim strCategory As String = Nothing
        Dim strGroupHeader As String = Nothing
        Dim blnNewGroup As Boolean
        Dim strSubGroup As String = Nothing
        Dim strPriceFormat As String

        'If UCase(strGroupBy) = "CATEGORY" Then strGroupBy = "CATEGORYNAME" 'JLC 31.01.2006
        Dim i As Integer
        For i = 1 To length
            column = dtShoppingList.Columns(i - 1)
            If column.ColumnName.ToUpper = strGroupBy.ToUpper Then
                dtShoppingList.Columns(i - 1).ColumnName = "GROUP"
                Exit For
            End If
        Next


        Cursor.Current = Cursors.WaitCursor

        sf = New StringFormat(StringFormatFlags.NoClip)

        Try
            'If G_ReportOptions.blnPictureOneRight Then
            '    fntReportTitle = New Font(strFontName, 14, FontStyle.Bold)
            'Else
            '    fntReportTitle = New Font(strFontName, 16, FontStyle.Bold)
            'End If

            fntReportTitle = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'VRP 05.11.2007
            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntBold = New Font(strFontName, sgFontSize, FontStyle.Bold)
            fntItalic = New Font(strFontName, sgFontSize, FontStyle.Bold Or FontStyle.Italic)
            fntFooter = New Font(strFontName, 8, FontStyle.Bold)

        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try

        xrLblEGS.Font = fntFooter

        Try
            If dtShoppingList.DefaultView.Count > 0 Then
                With Me
                    Me.DataMember = dtShoppingList.TableName.ToString
                    Me.DataSource = dtShoppingList

                    'AGL 2013.04.10 - 5075
                    .Detail.Controls.Clear()

                    '                    .XrLabel1.Visible = False

                    'Papersize
                    '------------
                    .PaperKind = Printing.PaperKind.Custom
                    .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

                    'Margins
                    '------------
                    .Margins.Left = CInt(dblLeftMargin)
                    .Margins.Top = CInt(dblTopMargin)
                    .Margins.Bottom = CInt(dblBottomMargin)
                    .Margins.Right = CInt(dblRightMargin)

                    'Orientation
                    '-----------
                    If blLandscape Then
                        .Landscape = True
                        intAvailableWidth = .PageHeight
                    Else
                        .Landscape = False
                        intAvailableWidth = .PageWidth
                    End If

                    intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin)

                    .xrLinePF.Left = 0
                    .xrLinePF.Width = intAvailableWidth
                    .xrPIPageNumber.Left = intAvailableWidth / 2
                    .xrPIPageNumber.Width = intAvailableWidth / 2
                    .xrPIPageNumber.Font = fntBold
                    .xrLblEGS.Left = 0
                    .xrLblEGS.Width = intAvailableWidth / 2
                    .xrLblEGS.Font = fntBold
                    .xrPIPageNumber.Format = clang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

                    If NoPrintLines Then
                        .xrLinePF.Visible = False
                    End If
                    subReportFooter(intAvailableWidth, strFontName)

                    intCurrentX = 0
                    intCurrentY = 0

                    'Report Title
                    strReportTitle = clang.GetString(clsEGSLanguage.CodeType.Shoppinglist) & " - " & CStrDB(dtShoppingList.Rows(0)("ShoppinglistName")) 'VRP 02.09.2008
                    intTextHeight = ReportingTextUtils.MeasureText(strReportTitle, fntReportTitle, intAvailableWidth, sf, Me.Padding).Height
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strReportTitle, fntReportTitle, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})

                    intCurrentY += intTextHeight
                    Dim strGrp As String
                    If blnWithGroup Then
                        intTextHeight = ReportingTextUtils.MeasureText(strGroupBy, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                        If strGroupBy = "CategoryName" Then
                            strGrp = clang.GetString(clsEGSLanguage.CodeType.Category)
                        ElseIf strGroupBy = "Supplier" Then
                            strGrp = clang.GetString(clsEGSLanguage.CodeType.Supplier)
                        Else
                            strGrp = strGroupBy
                        End If
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Group) & ": " & strGrp, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft)}) '"Group by "
                        intCurrentY += intTextHeight
                    End If


                    'Sub Report Header
                    intTextHeight = ReportingTextUtils.MeasureText(strSubHeading, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strSubHeading, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft)})


                    'Get Column Width of wach header
                    sf = New StringFormat(StringFormatFlags.DirectionVertical)
                    L_lngCol(1) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Price), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace    'Price  --- ftb(1290)
                    L_lngCol(2) = ReportingTextUtils.MeasureText("/", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace        'Unit

                    L_lngCol(3) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityGross), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Gross Qty --- ftb(132736)
                    L_lngCol(4) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityNet), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Net Qty --- ftb(132614)
                    L_lngCol(5) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace    'Unit  --- ftb(5100)

                    If OneOrMetricImperial Then
                        L_lngCol(6) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityGross), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Gross Qty --- ftb(132736)
                        L_lngCol(7) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityNet), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Net Qty --- ftb(132614)
                    Else
                        L_lngCol(6) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Gross Qty --- ftb(132736)
                        L_lngCol(7) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Net Qty --- ftb(132614)
                    End If

                    L_lngCol(8) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace    'Unit  --- ftb(5100)
                    L_lngCol(9) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Amount), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace   'Amount  --- ftb(5720)
                    L_lngCol(10) = ReportingTextUtils.MeasureText("WWW", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    L_lngCol(11) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Number), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace       'Number --- ftb(5500)

                    For Each drvReport In dtShoppingList.DefaultView
                        dblPrice = 0
                        dblUnitfactor = 0
                        dblMetUnitfactor = 0
                        dblImpUnitfactor = 0
                        dblQty = 0
                        dblGross = 0
                        dblMetQty = 0
                        dblMetGross = 0
                        dblImpQty = 0
                        dblImpGross = 0

                        strPriceFormat = CType(drvReport("curFormat"), String)
                        sgType = CType(drvReport("Type"), Single)
                        strName = CType(drvReport("Name"), String)
                        strCurrency = ""
                        If Not IsDBNull(drvReport("Currency")) Then
                            strCurrency = CType(drvReport("Currency"), String)
                        End If
                        'strCurrency = CType(drvReport("Currency"), String)
                        strNumber = ""
                        If Not IsDBNull(drvReport("Number")) Then
                            strNumber = CType(drvReport("Number"), String)
                        End If
                        'strNumber = IIf(IsDBNull(drvReport("Number")), "", CType(drvReport("Number"), String))
                        strNumber = fctNumber2Text(strNumber)

                        dblPrice = 0
                        If Not IsDBNull(drvReport("Price")) Then
                            dblPrice = CType(drvReport("Price"), Double)
                        End If
                        'dblPrice = IIf(IsDBNull(drvReport("Price")), 0, CType(drvReport("Price"), Double))

                        If sgType = ID_Merchandise Then
                            dblUnitfactor = 0
                            If Not IsDBNull(drvReport("UnitFactor")) Then
                                dblUnitfactor = CType(drvReport("UnitFactor"), Double)
                            End If
                            'dblUnitfactor = IIf(IsDBNull(drvReport("UnitFactor")), 0, CType(drvReport("UnitFactor"), Double))
                            strUnitFormat = drvReport("unitformat").ToString
                        Else
                            dblUnitfactor = 1
                            strUnitFormat = G_FormatTwoDecimal
                        End If

                        'JTOC 04.12.2012
                        'If GetLicenseClientCode() = clsLicense.enumApp.USA Then

                        'End If
                        If Not IsDBNull(drvReport("NetQty")) Then dblQty = CType(drvReport("NetQty"), Double)
                        If Not IsDBNull(drvReport("GrossQty")) Then dblGross = CType(drvReport("GrossQty"), Double)

                        If Not IsDBNull(drvReport("MetricNetQty")) Then dblMetQty = CType(drvReport("MetricNetQty"), Double) 'anm 12-28-2015
                        If Not IsDBNull(drvReport("MetricGrossQty")) Then dblMetGross = CType(drvReport("MetricGrossQty"), Double) 'anm 12-28-2015
                        If Not IsDBNull(drvReport("ImperialNetQty")) Then dblImpQty = CType(drvReport("ImperialNetQty"), Double) 'anm 12-28-2015
                        If Not IsDBNull(drvReport("ImperialGrossQty")) Then dblImpGross = CType(drvReport("ImperialGrossQty"), Double) 'anm 12-28-2015

                        dblfactor = 1
                        If Not IsDBNull(drvReport("PriceUnitFactor")) Then
                            dblfactor = CType(drvReport("PriceUnitFactor"), Double)
                        End If
                        'dblfactor = IIf(IsDBNull(drvReport("PriceUnitFactor")), 1, CType(drvReport("PriceUnitFactor"), Double))

                        dblAmount = dblPrice * (dblGross * dblUnitfactor / dblfactor)
                        If Not IsDBNull(drvReport("Amount")) Then dblAmount = CType(drvReport("Amount"), Double)

                        dblMetImpAmount = IIf(dblMetGross = 0, dblPrice * (dblImpGross * dblImpUnitfactor / dblfactor), dblPrice * (dblMetGross * dblMetUnitfactor / dblfactor))

                        'If Not IsDBNull(drvReport("MetImpItemCost")) Then dblMetImpAmount = CType(drvReport("MetImpItemCost"), Double)'anm 12-28-2015

                        If sgType = ID_Merchandise Then
                            strUnitName = Trim(drvReport("Unit").ToString)
                            strPriceUnit = ""
                            If Not IsDBNull(drvReport("PriceUnit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                            strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'anm 12-28-2015
                            strImpUnitName = Trim(drvReport("ImperialUnit").ToString) 'anm 12-28-2015
                        Else
                            strUnitName = Trim(drvReport("Unit").ToString)
                            strPriceUnit = ""
                            If Not IsDBNull(drvReport("Unit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                            strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'anm 12-28-2015
                            strImpUnitName = Trim(drvReport("ImperialUnit").ToString) 'anm 12-28-2015
                        End If

                        If blnIncludeGrossQty And blnIncludeNetQty Then


                            If OneOrMetricImperial = False Then
                                strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                    L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If

                                strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                    L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If

                                strX = strUnitName

                                If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                    L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                            Else

                                If OneOrMetricImperialQuantityBasis = 2 Then '------Imperial----------
                                    strX = fctFormatNumericQuantity(dblImpGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                        L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = fctFormatNumericQuantity(dblImpQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(4) Then
                                        L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strImpUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                        L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    L_lngCol(6) = 0
                                    L_lngCol(7) = 0
                                    L_lngCol(8) = 0

                                ElseIf OneOrMetricImperialQuantityBasis = 1 Then '------Metric----------
                                    L_lngCol(3) = 0
                                    L_lngCol(4) = 0
                                    L_lngCol(5) = 0


                                    strX = fctFormatNumericQuantity(dblMetGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                        L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = fctFormatNumericQuantity(dblMetQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                        L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strMetUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                        L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                Else
                                    strX = fctFormatNumericQuantity(dblImpGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                        L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = fctFormatNumericQuantity(dblImpQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(4) Then
                                        L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strImpUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                        L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = fctFormatNumericQuantity(dblMetGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                        L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = fctFormatNumericQuantity(dblMetQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                        L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strMetUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                        L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                End If
                            End If


                        ElseIf blnIncludeGrossQty Then

                            If OneOrMetricImperial = False Then
                                strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                    L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                                L_lngCol(7) = 0
                                strX = strUnitName

                                If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                    L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                            Else

                                If OneOrMetricImperialQuantityBasis = 2 Then '------Imperial----------
                                    strX = fctFormatNumericQuantity(dblImpGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                        L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                    L_lngCol(4) = 0
                                    strX = strImpUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                        L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    L_lngCol(6) = 0
                                    L_lngCol(7) = 0
                                    L_lngCol(8) = 0

                                ElseIf OneOrMetricImperialQuantityBasis = 1 Then '------Metric----------
                                    L_lngCol(3) = 0
                                    L_lngCol(4) = 0
                                    L_lngCol(5) = 0


                                    strX = fctFormatNumericQuantity(dblMetGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                        L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                    L_lngCol(7) = 0
                                    strX = strMetUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                        L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                Else
                                    strX = fctFormatNumericQuantity(dblImpGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                        L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                    L_lngCol(4) = 0
                                    strX = strImpUnitName
                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                        L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = fctFormatNumericQuantity(dblMetGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                        L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                    L_lngCol(7) = 0
                                    strX = strMetUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                        L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                End If
                            End If
                        ElseIf blnIncludeNetQty Then
                            If OneOrMetricImperial = False Then
                                L_lngCol(6) = 0
                                strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                    L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If

                                strX = strUnitName

                                If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                    L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                End If
                            Else

                                If OneOrMetricImperialQuantityBasis = 2 Then '------Imperial----------
                                    L_lngCol(3) = 0
                                    strX = fctFormatNumericQuantity(dblImpQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(4) Then
                                        L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strImpUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                        L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    L_lngCol(6) = 0
                                    L_lngCol(7) = 0
                                    L_lngCol(8) = 0

                                ElseIf OneOrMetricImperialQuantityBasis = 1 Then '------Metric----------
                                    L_lngCol(3) = 0
                                    L_lngCol(4) = 0
                                    L_lngCol(5) = 0
                                    L_lngCol(6) = 0

                                    strX = fctFormatNumericQuantity(dblMetQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                        L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strMetUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                        L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                Else
                                    L_lngCol(3) = 0
                                    strX = fctFormatNumericQuantity(dblImpQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(4) Then
                                        L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strImpUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                        L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    L_lngCol(6) = 0
                                    strX = fctFormatNumericQuantity(dblMetQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                        L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If

                                    strX = strMetUnitName

                                    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                        L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                    End If
                                End If
                            End If
                        Else
                            L_lngCol(3) = 0
                            L_lngCol(4) = 0
                            L_lngCol(5) = 0
                            L_lngCol(6) = 0
                            L_lngCol(7) = 0
                            L_lngCol(8) = 0
                            L_lngCol(9) = 0
                            L_lngCol(10) = 0
                            L_lngCol(11) = 0
                            L_lngCol(12) = 0
                        End If

                        If blnIncludePrice Then
                            strX = Format(dblPrice, strPriceFormat)
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(1) Then
                                L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            strX = "/" & strPriceUnit
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(2) Then
                                L_lngCol(2) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            'JTOC 09.06.2013
                            If OneOrMetricImperial Then
                                strX = Format(dblMetImpAmount, strPriceFormat)
                            Else
                                strX = Format(dblAmount, strPriceFormat)
                            End If

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            strX = strCurrency
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(1) = 0
                            L_lngCol(2) = 0
                            L_lngCol(6) = 0
                            L_lngCol(7) = 0
                            L_lngCol(9) = 0
                            L_lngCol(10) = 0
                        End If

                        If blnIncludeNumber Then
                            strX = strNumber
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(8) = 0
                        End If

                    Next

                    L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(2))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(3))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(4))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(5))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(6))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(7))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(8))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(9))
                    L_lngNameW = Math.Abs(L_lngNameW - (2 * L_lngCol(10)))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(11))

                    If OneOrMetricImperial Then
                        If OneOrMetricImperialQuantityBasis = 2 Then
                            L_lngNameW += 100
                        End If
                    Else
                        L_lngNameW += 250

                    End If


                    sf = New StringFormat(StringFormatFlags.NoClip)
                    intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Number), fntRegular, L_lngCol(11), sf, Me.Padding).Height '"Number"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Products), fntRegular, L_lngNameW, sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Products), fntRegular, L_lngNameW, sf, Me.Padding).Height
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Price), fntRegular, L_lngCol(1), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Price), fntRegular, L_lngCol(1), sf, Me.Padding).Height '"Price"

                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityGross), fntRegular, L_lngCol(3), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityGross), fntRegular, L_lngCol(3), sf, Me.Padding).Height '"Imperial Gross Qty"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityNet), fntRegular, L_lngCol(4), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityNet), fntRegular, L_lngCol(4), sf, Me.Padding).Height '"Imperial Net Qty"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntRegular, L_lngCol(5), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntRegular, L_lngCol(5), sf, Me.Padding).Height '"Unit"

                    If OneOrMetricImperial Then
                        If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityGross), fntRegular, L_lngCol(6), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityGross), fntRegular, L_lngCol(3), sf, Me.Padding).Height '"Gross Qty"
                        If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityNet), fntRegular, L_lngCol(7), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityNet), fntRegular, L_lngCol(4), sf, Me.Padding).Height '"Net Qty"
                    Else
                        If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntRegular, L_lngCol(6), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntRegular, L_lngCol(3), sf, Me.Padding).Height '"Gross Qty"
                        If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntRegular, L_lngCol(7), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntRegular, L_lngCol(4), sf, Me.Padding).Height '"Net Qty"
                    End If


                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntRegular, L_lngCol(8), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntRegular, L_lngCol(5), sf, Me.Padding).Height '"Unit"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Amount), fntRegular, L_lngCol(9), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Amount), fntRegular, L_lngCol(6), sf, Me.Padding).Height '"Amount"



                    intCurrentY += (intTextHeight * 2)
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
                    intCurrentY += 10

                    If blnIncludeNumber Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Number), fntBold, intCurrentX, intCurrentY, L_lngCol(11) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)}) 'Number
                        intCurrentX += L_lngCol(11)
                    End If

                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Products), fntBold, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)}) '"Merchandise" 
                    intCurrentX += L_lngNameW

                    If blnIncludePrice Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel("", fntBold, intCurrentX, intCurrentY, L_lngCol(7) - 50, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True, , True)}) '"Price"
                        intCurrentX += L_lngCol(7) - 50

                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Price), fntBold, intCurrentX, intCurrentY, L_lngCol(1), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True, , True)}) '"Price"
                        intCurrentX += L_lngCol(1) + L_lngCol(2)
                    End If

                    If blnIncludeGrossQty Then
                        If OneOrMetricImperial Then
                            If OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityGross), fntBold, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Gross Qty"
                                intCurrentX += L_lngCol(3)
                            End If
                        End If
                    End If

                    If blnIncludeNetQty Then
                        If OneOrMetricImperial Then
                            If OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.ImperialQuantityNet), fntBold, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Net Qty"
                                intCurrentX += L_lngCol(4)
                            End If
                        End If

                    End If


                    If blnIncludeNetQty Or blnIncludeGrossQty Then
                        If OneOrMetricImperial Then
                            If OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Unit), fntBold, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Net Qty"
                                intCurrentX += L_lngCol(5)
                            End If
                        End If

                    End If


                    If blnIncludeGrossQty Then
                        If OneOrMetricImperial Then
                            If OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityGross), fntBold, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Net Qty"
                                intCurrentX += L_lngCol(6)
                            End If
                        Else
                            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntBold, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Net Qty"
                            intCurrentX += L_lngCol(6)
                        End If
                    End If


                    If blnIncludeNetQty Then
                        If OneOrMetricImperial Then
                            If OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.MetricQuantityNet), fntBold, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Net Qty"
                                intCurrentX += L_lngCol(7)
                            End If
                        Else
                            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntBold, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Net Qty"
                            intCurrentX += L_lngCol(8)
                        End If
                    End If

                    If blnIncludeGrossQty Or blnIncludeNetQty Then
                        If OneOrMetricImperial Then
                            If OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Unit), fntBold, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)}) '"Unit"
                                intCurrentX += L_lngCol(8)
                            End If
                        Else
                            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Unit), fntBold, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)}) '"Unit"
                            intCurrentX += L_lngCol(8)
                        End If
                    End If

                    If OneOrMetricImperial Then
                        If OneOrMetricImperialQuantityBasis = 2 Then
                            intCurrentX += 15
                        End If
                    Else
                        intCurrentX += 15
                    End If
                    If blnIncludePrice Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Amount), fntBold, intCurrentX, intCurrentY, L_lngCol(9) + 20, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True, , True)}) '"Amount"
                    End If

                    'intCurrentY -= 2
                    '.PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel("", fntBold, 0, intCurrentY, intAvailableWidth, 20, DevExpress.XtraPrinting.TextAlignment.TopRight, , , , True)})

                    intCurrentX = 0
                    intCurrentY += intTextHeight
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})

                    Dim intMaxHeight As Integer
                    Dim sgFontSizeTemp As Single
                    Dim intTextHeightTemp As Integer
                    Dim intSwitch As Integer

                    intCurrentY = 10
                    intMaxHeight = ReportingTextUtils.MeasureText("A", fntRegular, intAvailableWidth, sf, Me.Padding).Height
                    sf = New StringFormat(StringFormatFlags.MeasureTrailingSpaces)

                    Dim foundRows() As DataRow
                    strGroup = Nothing
                    If blnWithGroup Then   'With Grouping
Repeat:
                        If intSwitch = 0 Then
                            If dtShoppingList.Columns.Contains(G_ReportOptions.strSortBy) Then
                                foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            Else
                                foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, name ASC")
                            End If

                            'If G_ReportOptions.strSortBy = "CategoryName" Or G_ReportOptions.strSortBy = "Supplier" Then
                            '    If G_ReportOptions.strGroupBy = "CategoryName" And G_ReportOptions.strSortBy = "Supplier" Then
                            '        foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC,name ASC")
                            '    ElseIf G_ReportOptions.strGroupBy = "Supplier" And G_ReportOptions.strSortBy = "CategoryName" Then
                            '        foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            '    Else
                            '        foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, name ASC")
                            '    End If
                            'Else
                            '    'foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            '    foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, Name ASC")
                            'End If
                            intSwitch = 1
                        Else

                            If dtShoppingList.Columns.Contains(G_ReportOptions.strSortBy) Then
                                foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            Else
                                foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, name ASC")
                            End If

                            'If G_ReportOptions.strSortBy = "CategoryName" Or G_ReportOptions.strSortBy = "Supplier" Then
                            '    foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, name ASC, Name ASC")
                            'Else
                            '    'foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            '    foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, Name ASC")
                            'End If
                            intSwitch = 0
                            strGroup = Nothing
                        End If

                        For intRow As Integer = 0 To foundRows.GetUpperBound(0)
                            'Console.WriteLine(foundRows(intRow)("Group"))
                            'Console.WriteLine(foundRows(intRow)("Name"))

                            intCurrentX = 0
                            dblPrice = 0
                            dblUnitfactor = 0
                            dblQty = 0
                            dblGross = 0
                            dblMetUnitfactor = 0 'JTOC 09.09.2013
                            dblImpUnitfactor = 0 'JTOC 09.09.2013
                            dblMetQty = 0 'JTOC 09.09.2013
                            dblMetGross = 0 'JTOC 09.09.2013
                            dblImpQty = 0 'JTOC 09.09.2013
                            dblImpGross = 0 'JTOC 09.09.2013

                            strPriceFormat = CType(foundRows(intRow)("curFormat"), String)
                            sgType = CType(foundRows(intRow)("Type"), Single)
                            strName = CType(foundRows(intRow)("Name"), String)
                            strCurrency = ""
                            If Not IsDBNull(drvReport("Currency")) Then
                                strCurrency = CType(drvReport("Currency"), String)
                            End If
                            strNumber = ""
                            If Not IsDBNull(foundRows(intRow)("Number")) Then strNumber = CType(foundRows(intRow)("Number"), String)
                            strNumber = fctNumber2Text(strNumber)

                            dblPrice = 0
                            If Not IsDBNull(foundRows(intRow)("Price")) Then dblPrice = CType(foundRows(intRow)("Price"), Double)
                            dblPrice = Format(dblPrice, strPriceFormat)

                            If sgType = ID_Merchandise Then
                                dblUnitfactor = 0
                                If Not IsDBNull(foundRows(intRow)("UnitFactor")) Then dblUnitfactor = CType(foundRows(intRow)("UnitFactor"), Double)
                                strUnitFormat = foundRows(intRow)("UnitFormat").ToString
                            Else
                                dblUnitfactor = 1
                                strUnitFormat = G_FormatTwoDecimal
                            End If

                            If Not IsDBNull(foundRows(intRow)("NetQty")) Then dblQty = CType(foundRows(intRow)("NetQty"), Double)
                            If Not IsDBNull(foundRows(intRow)("GrossQty")) Then dblGross = CType(foundRows(intRow)("GrossQty"), Double)

                            If Not IsDBNull(foundRows(intRow)("MetricNetQty")) Then dblMetQty = CType(foundRows(intRow)("MetricNetQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            If Not IsDBNull(foundRows(intRow)("MetricGrossQty")) Then dblMetGross = CType(foundRows(intRow)("MetricGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            If Not IsDBNull(foundRows(intRow)("ImperialNetQty")) Then dblImpQty = CType(foundRows(intRow)("ImperialNetQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            If Not IsDBNull(foundRows(intRow)("ImperialGrossQty")) Then dblImpGross = CType(foundRows(intRow)("ImperialGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015

                            dblfactor = 1
                            If Not IsDBNull(foundRows(intRow)("PriceUnitFactor")) Then dblfactor = CType(foundRows(intRow)("PriceUnitFactor"), Double)

                            dblAmount = dblPrice * (dblGross * dblUnitfactor / dblfactor)
                            If Not IsDBNull(foundRows(intRow)("Amount")) Then dblAmount = CType(foundRows(intRow)("Amount"), Double)

                            'JTOC 09.09.2013
                            dblMetImpAmount = IIf(dblMetGross = 0, dblPrice * (dblImpGross * dblImpUnitfactor / dblfactor), dblPrice * (dblMetGross * dblMetUnitfactor / dblfactor))
                            'If Not IsDBNull(foundRows(intRow)("MetImpItemCost")) Then dblMetImpAmount = CType(foundRows(intRow)("MetImpItemCost"), Double) 'anm 12-28-2015


                            If sgType = ID_Merchandise Then
                                strUnitName = Trim(foundRows(intRow)("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(foundRows(intRow)("PriceUnit")) Then strPriceUnit = foundRows(intRow)("PriceUnit").ToString

                                strMetUnitName = Trim(foundRows(intRow)("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                strImpUnitName = Trim(foundRows(intRow)("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                            Else
                                strUnitName = Trim(foundRows(intRow)("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(foundRows(intRow)("Unit")) Then strPriceUnit = foundRows(intRow)("Unit").ToString

                                strMetUnitName = Trim(foundRows(intRow)("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                strImpUnitName = Trim(foundRows(intRow)("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                            End If

                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            sgFontSizeTemp = sgFontSize

                            If intSwitch = 1 Then
                                If UCase(strGroup) <> UCase(CType(foundRows(intRow)("Group"), String)) Then
                                    If Not IsNothing(strGroup) Then
                                        intCurrentY += intTextHeight
                                    End If

                                    strGroup = CType(foundRows(intRow)("Group"), String)
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroup, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                                    intCurrentY += intTextHeight + 10
                                    strGroupHeader = Nothing
                                End If

                                '--------------------------------------------------------
                                blnNewGroup = False
                                If G_ReportOptions.strSortBy = "CategoryName" Then
                                    If G_ReportOptions.strGroupBy <> "CategoryName" Then
                                        strX = CType(foundRows(intRow)("CategoryName"), String)
                                        If UCase(strGroupHeader) <> UCase(strX) Then
                                            strGroupHeader = strX
                                            strX = clang.GetString(clsEGSLanguage.CodeType.Category)
                                            blnNewGroup = True
                                        End If
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "Supplier" Then
                                    If G_ReportOptions.strGroupBy <> "Supplier" Then

                                        'Per Supplier
                                        strX = CType(foundRows(intRow)("Supplier"), String)
                                        If UCase(strGroupHeader) <> UCase(strX) Then
                                            strGroupHeader = strX
                                            strX = clang.GetString(clsEGSLanguage.CodeType.Supplier)
                                            blnNewGroup = True
                                        End If
                                    End If

                                ElseIf G_ReportOptions.strSortBy = "Price" And Not blnIncludePrice Then
                                    strX = strCurrency & " " & Format(dblPrice, strPriceFormat) & "/" & strPriceUnit
                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Price)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "GrossQty" And Not blnIncludeGrossQty Then
                                    'JTOC 09.09.2013
                                    If G_ReportOptions.blnUseMetricImperial Then
                                        strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    Else
                                        strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    End If

                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Gross_Qty)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "NetQty" And Not blnIncludeNetQty Then
                                    'JTOC 09.09.2013
                                    If G_ReportOptions.blnUseMetricImperial Then
                                        strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpQty, dblMetQty), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    Else
                                        strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    End If

                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Net_Qty)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "Amount" And Not blnIncludePrice Then
                                    'JTOC 09.09.2013
                                    If G_ReportOptions.blnUseMetricImperial Then
                                        strX = strCurrency & " " & Format(dblMetImpAmount, strPriceFormat)
                                    Else
                                        strX = strCurrency & " " & Format(dblAmount, strPriceFormat)
                                    End If

                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Amount)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "Number" And Not blnIncludeNumber Then
                                    strX = strNumber
                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Number)
                                        blnNewGroup = True
                                    End If
                                End If


                                If blnNewGroup Then
                                    intCurrentY += 5
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & ": " & strGroupHeader, fntItalic, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    intCurrentY += intTextHeight + 5
                                End If
                                '------------------------------------------------------

                            Else
                                If strGroup = Nothing Then
                                    intCurrentY += intTextHeight
                                    strGroup = "***No " & strGroupBy & "***"
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroup, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                                    intCurrentY += intTextHeight + 10
                                End If
                            End If

                            If blnIncludeNumber Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strNumber, fntRegular, intCurrentX, intCurrentY, L_lngCol(11) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(11)
                            End If

                            intTextHeightTemp = CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                            intTextHeight = intTextHeightTemp
                            If G_ReportOptions.blShrinkToFit Then
                                Do While Not intMaxHeight >= CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                                    sgFontSizeTemp -= 1
                                    fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                                Loop
                                intTextHeight = intMaxHeight
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            Else
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                            End If

                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            intCurrentX += L_lngNameW

                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace - 50, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(7) - 50

                                strX = Format(dblPrice, strPriceFormat)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(1) - intColumnSpace

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel("/" & strPriceUnit, fntRegular, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(2) + intColumnSpace
                            End If

                            'bann
                            If blnIncludeGrossQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblImpGross, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(3)
                                    End If
                                End If
                            End If

                            If blnIncludeNetQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblImpQty, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(4)
                                    End If
                                End If

                            End If

                            If blnIncludeNetQty Or blnIncludeGrossQty Then
                                If OneOrMetricImperial And OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                    strX = Trim(foundRows(intRow)("ImperialUnit").ToString)
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                    intCurrentX += L_lngCol(5)

                                End If
                            End If

                            If blnIncludeGrossQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblMetGross, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(6)
                                    End If
                                Else
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                    intCurrentX += L_lngCol(6)
                                End If
                            End If


                            If blnIncludeNetQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblMetQty, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(7)
                                    End If
                                Else
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                    intCurrentX += L_lngCol(8)
                                End If
                            End If

                            If blnIncludeGrossQty Or blnIncludeNetQty Then

                                'JTOC 09.10.2013
                                'If G_ReportOptions.blnUseMetricImperial Then
                                '    strX = IIf(UCase((LTrim(strMetUnitName))) = "N/A", strImpUnitName, strMetUnitName)
                                'Else
                                '    strX = strUnitName
                                'End If
                                If OneOrMetricImperial And OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                    strX = Trim(drvReport("MetricUnit").ToString)

                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    intCurrentX += L_lngCol(9)
                                ElseIf OneOrMetricImperialQuantityBasis = 2 Then
                                    'strX = " "
                                    '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    'intCurrentX += L_lngCol(9)
                                Else
                                    strX = strUnitName
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    intCurrentX += L_lngCol(9)
                                End If

                            End If

                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(10) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(10) - L_lngCol(11) + 16


                                If OneOrMetricImperial Then
                                    dblMetImpAmount = Trim(drvReport("MetimpItemCost").ToString)
                                    strX = Format(dblMetImpAmount, strPriceFormat)
                                Else
                                    dblAmount = Trim(drvReport("Amount").ToString)
                                    strX = Format(dblAmount, strPriceFormat)
                                End If

                                strX = Format(dblAmount, strPriceFormat)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(11) - intColumnSpace + 20, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                            End If

                            'intCurrentY += intTextHeight 
                            intCurrentY += GetLineSpace(intMaxHeight)
                        Next


                        If intSwitch = 1 Then
                            GoTo Repeat
                        End If

                    Else        'Without Grouping

                        For Each drvReport In dtShoppingList.DefaultView
                            intCurrentX = 0
                            dblPrice = 0
                            dblUnitfactor = 0
                            dblQty = 0
                            dblGross = 0
                            dblMetUnitfactor = 0 'JTOC 09.09.2013
                            dblImpUnitfactor = 0 'JTOC 09.09.2013
                            dblMetQty = 0 'JTOC 09.09.2013
                            dblMetGross = 0 'JTOC 09.09.2013
                            dblImpQty = 0 'JTOC 09.09.2013
                            dblImpGross = 0 'JTOC 09.09.2013

                            strPriceFormat = CType(drvReport("curFormat"), String)
                            sgType = CType(drvReport("Type"), Single)
                            strName = drvReport("Name").ToString
                            strCurrency = drvReport("Currency").ToString
                            strNumber = ""
                            If Not IsDBNull(drvReport("Number")) Then strNumber = CType(drvReport("Number"), String)
                            strNumber = fctNumber2Text(strNumber)

                            dblPrice = 0
                            If Not IsDBNull(drvReport("Price")) Then dblPrice = CType(drvReport("Price"), Double)
                            dblPrice = Format(dblPrice, strPriceFormat)

                            If sgType = ID_Merchandise Then
                                dblUnitfactor = 0
                                If Not IsDBNull(drvReport("UnitFactor")) Then dblUnitfactor = CType(drvReport("UnitFactor"), Double)
                                strUnitFormat = drvReport("unitformat").ToString
                            Else
                                dblUnitfactor = 1
                                strUnitFormat = G_FormatTwoDecimal
                            End If

                            If Not IsDBNull(drvReport("NetQty")) Then dblQty = CType(drvReport("NetQty"), Double)
                            If Not IsDBNull(drvReport("GrossQty")) Then dblGross = CType(drvReport("GrossQty"), Double)

                            If Not IsDBNull(drvReport("MetricNetQty")) Then dblMetQty = CType(drvReport("MetricNetQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            If Not IsDBNull(drvReport("MetricGrossQty")) Then dblMetGross = CType(drvReport("MetricGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            If Not IsDBNull(drvReport("ImperialNetQty")) Then dblImpQty = CType(drvReport("ImperialNetQty"), Double) 'JTOC 09.09.2013  'anm 12-28-2015
                            If Not IsDBNull(drvReport("ImperialGrossQty")) Then dblImpGross = CType(drvReport("ImperialGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015

                            dblfactor = 1
                            If Not IsDBNull(drvReport("PriceUnitFactor")) Then dblfactor = CType(drvReport("PriceUnitFactor"), Double)

                            dblAmount = dblPrice * (dblGross * dblUnitfactor / dblfactor)
                            If Not IsDBNull(drvReport("Amount")) Then dblAmount = CType(drvReport("Amount"), Double)

                            'JTOC 09.09.2013
                            dblMetImpAmount = IIf(dblMetGross = 0, dblPrice * (dblImpGross * dblImpUnitfactor / dblfactor), dblPrice * (dblMetGross * dblMetUnitfactor / dblfactor))
                            ' If Not IsDBNull(drvReport("MetImpItemCost")) Then dblMetImpAmount = CType(drvReport("MetImpItemCost"), Double)'anm 12-28-2015

                            If sgType = ID_Merchandise Then
                                strUnitName = Trim(drvReport("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(drvReport("PriceUnit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                                strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                strImpUnitName = Trim(drvReport("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015

                            Else
                                strUnitName = Trim(drvReport("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(drvReport("Unit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                                strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                strImpUnitName = Trim(drvReport("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015

                            End If



                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            sgFontSizeTemp = sgFontSize
                            blnNewGroup = False
                            If G_ReportOptions.strSortBy = "CategoryName" Then
                                strX = CType(drvReport("CategoryName"), String)
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Supplier" Then
                                'Per Supplier
                                strX = CType(drvReport("Supplier"), String)
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Price" And Not blnIncludePrice Then
                                strX = strCurrency & " " & Format(dblPrice, strPriceFormat) & "/" & strPriceUnit
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "GrossQty" And Not blnIncludeGrossQty Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                Else
                                    strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                End If

                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "NetQty" And Not blnIncludeNetQty Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpQty, dblMetQty), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                Else
                                    strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                End If
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Amount" And Not blnIncludePrice Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = strCurrency & " " & Format(dblMetImpAmount, strPriceFormat)
                                Else
                                    strX = strCurrency & " " & Format(dblAmount, strPriceFormat)
                                End If

                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Number" And Not blnIncludeNumber Then
                                strX = strNumber
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            End If


                            If blnNewGroup Then
                                intCurrentY += 5
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroupHeader, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                                intCurrentY += intTextHeight + 5
                            End If

                            If blnIncludeNumber Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strNumber, fntRegular, intCurrentX, intCurrentY, L_lngCol(11) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(11)
                            End If

                            If G_ReportOptions.blShrinkToFit Then
                                intTextHeightTemp = CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                                Do While Not intMaxHeight >= CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW, sf, Me.Padding).Height)
                                    sgFontSizeTemp -= 1
                                    fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                                Loop
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            Else
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                            End If


                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            intCurrentX += L_lngNameW

                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace - 50, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(7) - 50

                                strX = Format(dblPrice, strPriceFormat)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(1) - intColumnSpace

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel("/" & strPriceUnit, fntRegular, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(2) + intColumnSpace
                            End If

                            If blnIncludeGrossQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblImpGross, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(3)
                                    End If
                                End If


                            End If

                            If blnIncludeNetQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblImpQty, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(4)
                                    End If
                                End If

                            End If


                            If blnIncludeNetQty Or blnIncludeGrossQty Then
                                If OneOrMetricImperial And OneOrMetricImperialQuantityBasis = 2 Or OneOrMetricImperialQuantityBasis = 3 Then
                                    strX = Trim(drvReport("ImperialUnit").ToString)
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strImpUnitName, fntRegular, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                    intCurrentX += L_lngCol(5)

                                End If
                            End If


                            If blnIncludeGrossQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblMetGross, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(6)
                                    End If
                                Else
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                    intCurrentX += L_lngCol(6)
                                End If
                            End If


                            If blnIncludeNetQty Then
                                If OneOrMetricImperial Then
                                    If OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                        strX = Format(dblMetQty, strPriceFormat)
                                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                        intCurrentX += L_lngCol(7)
                                    End If
                                Else
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                    intCurrentX += L_lngCol(8)
                                End If
                            End If

                            If blnIncludeGrossQty Or blnIncludeNetQty Then

                                'JTOC 09.10.2013
                                'If G_ReportOptions.blnUseMetricImperial Then
                                '    strX = IIf(UCase((LTrim(strMetUnitName))) = "N/A", strImpUnitName, strMetUnitName)
                                'Else
                                '    strX = strUnitName
                                'End If
                                If OneOrMetricImperial And OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                    strX = Trim(drvReport("MetricUnit").ToString)

                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    intCurrentX += L_lngCol(9)
                                ElseIf OneOrMetricImperialQuantityBasis = 2 Then
                                    'strX = " "
                                    '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    'intCurrentX += L_lngCol(9)
                                Else
                                    strX = strUnitName
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    intCurrentX += L_lngCol(9)
                                End If


                                'If OneOrMetricImperial And OneOrMetricImperialQuantityBasis = 1 Or OneOrMetricImperialQuantityBasis = 3 Then
                                '    strX = Trim(drvReport("MetricUnit").ToString)
                                'Else
                                '    strX = strUnitName

                                'End If


                                '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                'intCurrentX += L_lngCol(9)
                            End If
                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(10) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(10) - L_lngCol(11) + 16

                                'JTOC 09.09.2013

                                If OneOrMetricImperial Then
                                    dblMetImpAmount = Trim(drvReport("MetimpItemCost").ToString)
                                    strX = Format(dblMetImpAmount, strPriceFormat)
                                Else
                                    dblAmount = Trim(drvReport("Amount").ToString)
                                    strX = Format(dblAmount, strPriceFormat)
                                End If

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(11) - intColumnSpace + 20, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                            End If
                            'intCurrentY += intTextHeight
                            intCurrentY += GetLineSpace(intMaxHeight)

                        Next
                    End If

                    intCurrentX = 0
                    intCurrentY += intTextHeight
                    Dim MaxHeight As Integer
                    Dim totalAmount As Double
                    intCurrentY += 10
                    MaxHeight = ReportingTextUtils.MeasureText("A", fntRegular, intAvailableWidth, sf, Me.Padding).Height

                    For Each drvReport In dtShoppingList.DefaultView
                        strPriceFormat = CType(drvReport("curFormat"), String)
                        strCurrency = drvReport("Currency").ToString
                        dblPrice = 0
                        If Not IsDBNull(drvReport("Price")) Then dblPrice = CType(drvReport("Price"), Double)
                        dblPrice = Format(dblPrice, strPriceFormat)
                        If Not IsDBNull(drvReport("GrossQty")) Then dblGross = CType(drvReport("GrossQty"), Double)
                        dblfactor = 1
                        If Not IsDBNull(drvReport("PriceUnitFactor")) Then dblfactor = CType(drvReport("PriceUnitFactor"), Double)
                        dblAmount = dblPrice * (dblGross * dblUnitfactor / dblfactor)
                        If Not IsDBNull(drvReport("Amount")) Then dblAmount = CType(drvReport("Amount"), Double)
                        totalAmount += dblAmount
                    Next
                    fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)

                    'If blnIncludePrice Then

                    '    If ReportingTextUtils.MeasureText(strCurrency, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                    '        L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    '    End If
                    '    strX = Format(totalAmount, strPriceFormat)
                    '    If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                    '        L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    '    End If

                    '    L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(7))
                    '    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(6))

                    '    strX = " "
                    '    L_lngCol(5) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Total), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    '    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Total) & ":", fntBold, intCurrentX, intCurrentY, L_lngNameW + 18, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    '    intCurrentX += L_lngNameW + 18

                    '    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntBold, intCurrentX - 5, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    '    intCurrentX += L_lngCol(7)

                    '    strX = Format(totalAmount, strPriceFormat)
                    '    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX - 18, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    'End If

                End With
            End If
            Cursor.Current = Cursors.Arrow
            dtShoppingList.Reset() 'VRP 11.03.2008
            Return Me
        Catch ex As Exception
            Dim x As String = ex.Message()
        End Try
    End Function
    Function fctPrintShoppingListWithoutImp(ByVal dtShoppingList As DataTable, ByVal strSubHeading As String,
                                   ByVal blnWithGroup As Boolean, ByVal strGroupBy As String,
                                   ByVal blnIncludePrice As Boolean, ByVal blnIncludeGrossQty As Boolean, ByVal blnIncludeNetQty As Boolean,
                                   ByVal blnIncludeNumber As Boolean,
                                   ByVal strFontName As String, ByVal sgFontSize As Single,
                                   ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double,
                                   ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, Optional ByVal blLandscape As Boolean = False,
                                   Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16,
                                   Optional ByVal blnIncludeMetricGrossQty As Boolean = False, Optional ByVal blnIncludeMetricNetQty As Boolean = False,
                                   Optional ByVal blnIncludeImperialGrossQty As Boolean = False, Optional ByVal blnIncludeImperialNetQty As Boolean = False) As XtraReport 'VRP 15.11.2007

        Dim column As DataColumn
        Dim length As Integer = dtShoppingList.Columns.Count
        Dim clang As New clsEGSLanguage(G_ReportOptions.intPageLanguage)
        Dim drvReport As DataRowView

        Dim dblPrice As Double
        Dim dblQty As Double
        Dim dblMetQty As Double
        Dim dblImpQty As Double
        Dim dblUnitfactor As Double
        Dim dblMetUnitfactor As Double
        Dim dblImpUnitfactor As Double
        Dim dblGross As Double
        Dim dblMetGross As Double
        Dim dblImpGross As Double
        Dim dblfactor As Double
        'Dim dblMetfactor As Double
        'Dim dblImpfactor As Double
        Dim dblAmount As Double
        Dim dblMetImpAmount As Double

        Dim sgType As Single
        Dim strName As String
        Dim strNumber As String
        Dim strCurrency As String
        Dim strUnitFormat As String
        'Dim strMetUnitFormat As String
        'Dim strImpUnitFormat As String
        Dim strUnitName As String
        Dim strMetUnitName As String = ""
        Dim strImpUnitName As String = ""
        Dim strPriceUnit As String
        Dim strX As String = ""
        Dim strGroup As String

        Dim strSupplier As String = Nothing
        Dim strCategory As String = Nothing
        Dim strGroupHeader As String = Nothing
        Dim blnNewGroup As Boolean
        Dim strSubGroup As String = Nothing
        Dim strPriceFormat As String

        'If UCase(strGroupBy) = "CATEGORY" Then strGroupBy = "CATEGORYNAME" 'JLC 31.01.2006
        Dim i As Integer
        For i = 1 To length
            column = dtShoppingList.Columns(i - 1)
            If column.ColumnName.ToUpper = strGroupBy.ToUpper Then
                dtShoppingList.Columns(i - 1).ColumnName = "GROUP"
                Exit For
            End If
        Next


        Cursor.Current = Cursors.WaitCursor

        sf = New StringFormat(StringFormatFlags.NoClip)

        Try
            'If G_ReportOptions.blnPictureOneRight Then
            '    fntReportTitle = New Font(strFontName, 14, FontStyle.Bold)
            'Else
            '    fntReportTitle = New Font(strFontName, 16, FontStyle.Bold)
            'End If

            fntReportTitle = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'VRP 05.11.2007
            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntBold = New Font(strFontName, sgFontSize, FontStyle.Bold)
            fntItalic = New Font(strFontName, sgFontSize, FontStyle.Bold Or FontStyle.Italic)
            fntFooter = New Font(strFontName, 8, FontStyle.Bold)

        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try

        xrLblEGS.Font = fntFooter

        Try
            If dtShoppingList.DefaultView.Count > 0 Then
                With Me
                    Me.DataMember = dtShoppingList.TableName.ToString
                    Me.DataSource = dtShoppingList

                    'AGL 2013.04.10 - 5075
                    .Detail.Controls.Clear()

                    '                    .XrLabel1.Visible = False

                    'Papersize
                    '------------
                    .PaperKind = Printing.PaperKind.Custom
                    .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

                    'Margins
                    '------------
                    .Margins.Left = CInt(dblLeftMargin)
                    .Margins.Top = CInt(dblTopMargin)
                    .Margins.Bottom = CInt(dblBottomMargin)
                    .Margins.Right = CInt(dblRightMargin)

                    'Orientation
                    '-----------
                    If blLandscape Then
                        .Landscape = True
                        intAvailableWidth = .PageHeight
                    Else
                        .Landscape = False
                        intAvailableWidth = .PageWidth
                    End If

                    intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin)

                    .xrLinePF.Left = 0
                    .xrLinePF.Width = intAvailableWidth
                    .xrPIPageNumber.Left = intAvailableWidth / 2
                    .xrPIPageNumber.Width = intAvailableWidth / 2
                    .xrPIPageNumber.Font = fntBold
                    .xrLblEGS.Left = 0
                    .xrLblEGS.Width = intAvailableWidth / 2
                    .xrLblEGS.Font = fntBold
                    .xrPIPageNumber.Format = clang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

                    If NoPrintLines Then
                        .xrLinePF.Visible = False
                    End If
                    subReportFooter(intAvailableWidth, strFontName)

                    intCurrentX = 0
                    intCurrentY = 0

                    'Report Title
                    strReportTitle = clang.GetString(clsEGSLanguage.CodeType.Shoppinglist) & " - " & CStrDB(dtShoppingList.Rows(0)("ShoppinglistName")) 'VRP 02.09.2008
                    intTextHeight = ReportingTextUtils.MeasureText(strReportTitle, fntReportTitle, intAvailableWidth, sf, Me.Padding).Height
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strReportTitle, fntReportTitle, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})

                    intCurrentY += intTextHeight
                    Dim strGrp As String
                    If blnWithGroup Then
                        intTextHeight = ReportingTextUtils.MeasureText(strGroupBy, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                        If strGroupBy = "CategoryName" Then
                            strGrp = clang.GetString(clsEGSLanguage.CodeType.Category)
                        ElseIf strGroupBy = "Supplier" Then
                            strGrp = clang.GetString(clsEGSLanguage.CodeType.Supplier)
                        Else
                            strGrp = strGroupBy
                        End If
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Group) & ": " & strGrp, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft)}) '"Group by "
                        intCurrentY += intTextHeight
                    End If


                    'Sub Report Header
                    intTextHeight = ReportingTextUtils.MeasureText(strSubHeading, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strSubHeading, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft)})


                    'Get Column Width of wach header
                    sf = New StringFormat(StringFormatFlags.DirectionVertical)
                    L_lngCol(1) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Price), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace    'Price  --- ftb(1290)
                    L_lngCol(2) = ReportingTextUtils.MeasureText("/", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace        'Unit
                    L_lngCol(3) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Gross Qty --- ftb(132736)
                    L_lngCol(4) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace  'Net Qty --- ftb(132614)
                    L_lngCol(5) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace    'Unit  --- ftb(5100)
                    L_lngCol(6) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Amount), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace   'Amount  --- ftb(5720)
                    L_lngCol(7) = ReportingTextUtils.MeasureText("WWW", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    L_lngCol(8) = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Number), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace       'Number --- ftb(5500)

                    For Each drvReport In dtShoppingList.DefaultView
                        dblPrice = 0
                        dblUnitfactor = 0
                        dblMetUnitfactor = 0
                        dblImpUnitfactor = 0
                        dblQty = 0
                        dblGross = 0
                        dblMetQty = 0
                        dblMetGross = 0
                        dblImpQty = 0
                        dblImpGross = 0

                        strPriceFormat = CType(drvReport("curFormat"), String)
                        sgType = CType(drvReport("Type"), Single)
                        strName = CType(drvReport("Name"), String)
                        strCurrency = ""
                        If Not IsDBNull(drvReport("Currency")) Then
                            strCurrency = CType(drvReport("Currency"), String)
                        End If
                        'strCurrency = CType(drvReport("Currency"), String)
                        strNumber = ""
                        If Not IsDBNull(drvReport("Number")) Then
                            strNumber = CType(drvReport("Number"), String)
                        End If
                        'strNumber = IIf(IsDBNull(drvReport("Number")), "", CType(drvReport("Number"), String))
                        strNumber = fctNumber2Text(strNumber)

                        dblPrice = 0
                        If Not IsDBNull(drvReport("Price")) Then
                            dblPrice = CType(drvReport("Price"), Double)
                        End If
                        'dblPrice = IIf(IsDBNull(drvReport("Price")), 0, CType(drvReport("Price"), Double))

                        If sgType = ID_Merchandise Then
                            dblUnitfactor = 0
                            If Not IsDBNull(drvReport("UnitFactor")) Then
                                dblUnitfactor = CType(drvReport("UnitFactor"), Double)
                            End If
                            'dblUnitfactor = IIf(IsDBNull(drvReport("UnitFactor")), 0, CType(drvReport("UnitFactor"), Double))
                            strUnitFormat = drvReport("unitformat").ToString
                        Else
                            dblUnitfactor = 1
                            strUnitFormat = G_FormatTwoDecimal
                        End If

                        'JTOC 04.12.2012
                        'If GetLicenseClientCode() = clsLicense.enumApp.USA Then

                        'End If
                        If Not IsDBNull(drvReport("NetQty")) Then dblQty = CType(drvReport("NetQty"), Double)
                        If Not IsDBNull(drvReport("GrossQty")) Then dblGross = CType(drvReport("GrossQty"), Double)

                        '  If Not IsDBNull(drvReport("MetricNetQty")) Then dblMetQty = CType(drvReport("MetricNetQty"), Double) 'anm 12-28-2015


                        ' If Not IsDBNull(drvReport("MetricGrossQty")) Then dblMetGross = CType(drvReport("MetricGrossQty"), Double)  'anm 12-28-2015
                        'If Not IsDBNull(drvReport("ImperialNetQty")) Then dblImpQty = CType(drvReport("ImperialNetQty"), Double) 'anm 12-28-2015
                        'If Not IsDBNull(drvReport("ImperialGrossQty")) Then dblImpGross = CType(drvReport("ImperialGrossQty"), Double) 'anm 12-28-2015

                        dblfactor = 1
                        If Not IsDBNull(drvReport("PriceUnitFactor")) Then
                            dblfactor = CType(drvReport("PriceUnitFactor"), Double)
                        End If
                        'dblfactor = IIf(IsDBNull(drvReport("PriceUnitFactor")), 1, CType(drvReport("PriceUnitFactor"), Double))

                        dblAmount = dblPrice * (dblGross * dblUnitfactor / dblfactor)
                        If Not IsDBNull(drvReport("Amount")) Then dblAmount = CType(drvReport("Amount"), Double)

                        dblMetImpAmount = IIf(dblMetGross = 0, dblPrice * (dblImpGross * dblImpUnitfactor / dblfactor), dblPrice * (dblMetGross * dblMetUnitfactor / dblfactor))

                        'If Not IsDBNull(drvReport("MetImpItemCost")) Then dblMetImpAmount = CType(drvReport("MetImpItemCost"), Double)'anm 12-28-2015

                        If sgType = ID_Merchandise Then
                            strUnitName = Trim(drvReport("Unit").ToString)
                            strPriceUnit = ""
                            If Not IsDBNull(drvReport("PriceUnit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                            'strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'anm 12-28-2015
                            'strImpUnitName = Trim(drvReport("ImperialUnit").ToString) 'anm 12-28-2015
                        Else
                            strUnitName = Trim(drvReport("Unit").ToString)
                            strPriceUnit = ""
                            If Not IsDBNull(drvReport("Unit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                            'strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'anm 12-28-2015
                            'strImpUnitName = Trim(drvReport("ImperialUnit").ToString)'anm 12-28-2015
                        End If

                        If blnIncludeGrossQty And blnIncludeNetQty Then
                            'JTOC 09.06.2013
                            If G_ReportOptions.blnUseMetricImperial Then
                                '  strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType) 'anm 12-28-2015
                            Else
                                strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                            End If

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                            'JTOC 09.06.2013
                            If G_ReportOptions.blnUseMetricImperial Then
                                'strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpQty, dblMetQty), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType) 'anm 12-28-2015
                            Else
                                strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                            End If

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(4) Then
                                L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            'JTOC 09.10.2013
                            If G_ReportOptions.blnUseMetricImperial Then
                                'strX = IIf(UCase((LTrim(strMetUnitName))) = "N/A", strImpUnitName, strMetUnitName) 'anm 12-28-2015
                            Else
                                strX = strUnitName
                            End If

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        ElseIf blnIncludeGrossQty Then
                            'JTOC 09.06.2013
                            If G_ReportOptions.blnUseMetricImperial Then
                                strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                            Else
                                strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                            End If

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            L_lngCol(4) = 0

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        ElseIf blnIncludeNetQty Then
                            L_lngCol(3) = 0
                            'JTOC 09.06.2013
                            If G_ReportOptions.blnUseMetricImperial Then
                                strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                            Else
                                strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                            End If

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(3) Then
                                L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(5) Then
                                L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                        Else
                            L_lngCol(3) = 0
                            L_lngCol(4) = 0
                            L_lngCol(5) = 0
                        End If

                        If blnIncludePrice Then
                            strX = Format(dblPrice, strPriceFormat)
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(1) Then
                                L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            strX = "/" & strPriceUnit
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(2) Then
                                L_lngCol(2) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            'JTOC 09.06.2013
                            If G_ReportOptions.blnUseMetricImperial Then
                                strX = Format(dblMetImpAmount, strPriceFormat)
                            Else
                                strX = Format(dblAmount, strPriceFormat)
                            End If

                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(6) Then
                                L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If

                            strX = strCurrency
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(7) Then
                                L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(1) = 0
                            L_lngCol(2) = 0
                            L_lngCol(6) = 0
                            L_lngCol(7) = 0
                        End If

                        If blnIncludeNumber Then
                            strX = strNumber
                            If ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace >= L_lngCol(8) Then
                                L_lngCol(8) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            End If
                        Else
                            L_lngCol(8) = 0
                        End If

                    Next

                    L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(2))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(3))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(4))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(5))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(6))
                    L_lngNameW = Math.Abs(L_lngNameW - (2 * L_lngCol(7)))
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(8))


                    sf = New StringFormat(StringFormatFlags.NoClip)
                    intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Number), fntRegular, L_lngCol(8), sf, Me.Padding).Height '"Number"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Products), fntRegular, L_lngNameW, sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Products), fntRegular, L_lngNameW, sf, Me.Padding).Height
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Price), fntRegular, L_lngCol(1), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Price), fntRegular, L_lngCol(1), sf, Me.Padding).Height '"Price"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntRegular, L_lngCol(3), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntRegular, L_lngCol(3), sf, Me.Padding).Height '"Gross Qty"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntRegular, L_lngCol(4), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntRegular, L_lngCol(4), sf, Me.Padding).Height '"Net Qty"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntRegular, L_lngCol(5), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Unit), fntRegular, L_lngCol(5), sf, Me.Padding).Height '"Unit"
                    If intTextHeight < ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Amount), fntRegular, L_lngCol(6), sf, Me.Padding).Height Then intTextHeight = ReportingTextUtils.MeasureText(clang.GetString(clsEGSLanguage.CodeType.Amount), fntRegular, L_lngCol(6), sf, Me.Padding).Height '"Amount"



                    'intCurrentY += (intTextHeight * 2)
                    intCurrentY += intTextHeight + 5
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
                    intCurrentY += 10

                    If blnIncludeNumber Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Number), fntBold, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)}) 'Number
                        intCurrentX += L_lngCol(8)
                    End If

                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Products), fntBold, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)}) '"Merchandise" 
                    intCurrentX += L_lngNameW + L_lngCol(7)

                    If blnIncludePrice Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Price), fntBold, intCurrentX, intCurrentY, L_lngCol(1), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True, , True)}) '"Price"
                        intCurrentX += L_lngCol(1) + L_lngCol(2)
                    End If

                    If blnIncludeGrossQty Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Gross_Qty), fntBold, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Gross Qty"
                        intCurrentX += L_lngCol(3)
                    End If

                    If blnIncludeNetQty Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Net_Qty), fntBold, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)}) '"Net Qty"
                        intCurrentX += L_lngCol(4)
                    End If

                    If blnIncludeGrossQty Or blnIncludeNetQty Then
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Unit), fntBold, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)}) '"Unit"
                        intCurrentX += L_lngCol(5)
                    End If

                    If blnIncludePrice Then
                        intCurrentX += L_lngCol(7)
                        .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(clang.GetString(clsEGSLanguage.CodeType.Amount), fntBold, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True, , True)}) '"Amount"
                    End If

                    'intCurrentY -= 2
                    '.PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel("", fntBold, 0, intCurrentY, intAvailableWidth, 20, DevExpress.XtraPrinting.TextAlignment.TopRight, , , , True)})

                    intCurrentX = 0
                    intCurrentY += intTextHeight
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})

                    Dim intMaxHeight As Integer
                    Dim sgFontSizeTemp As Single
                    Dim intTextHeightTemp As Integer
                    Dim intSwitch As Integer

                    intCurrentY = 10
                    intMaxHeight = ReportingTextUtils.MeasureText("A", fntRegular, intAvailableWidth, sf, Me.Padding).Height
                    sf = New StringFormat(StringFormatFlags.MeasureTrailingSpaces)

                    Dim foundRows() As DataRow
                    strGroup = Nothing
                    If blnWithGroup Then   'With Grouping
Repeat:
                        If intSwitch = 0 Then
                            If dtShoppingList.Columns.Contains(G_ReportOptions.strSortBy) Then
                                foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            Else
                                foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, name ASC")
                            End If

                            'If G_ReportOptions.strSortBy = "CategoryName" Or G_ReportOptions.strSortBy = "Supplier" Then
                            '    If G_ReportOptions.strGroupBy = "CategoryName" And G_ReportOptions.strSortBy = "Supplier" Then
                            '        foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC,name ASC")
                            '    ElseIf G_ReportOptions.strGroupBy = "Supplier" And G_ReportOptions.strSortBy = "CategoryName" Then
                            '        foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            '    Else
                            '        foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, name ASC")
                            '    End If
                            'Else
                            '    'foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            '    foundRows = dtShoppingList.Select("Group <> ''", "[Group] ASC, Name ASC")
                            'End If
                            intSwitch = 1
                        Else

                            If dtShoppingList.Columns.Contains(G_ReportOptions.strSortBy) Then
                                foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            Else
                                foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, name ASC")
                            End If

                            'If G_ReportOptions.strSortBy = "CategoryName" Or G_ReportOptions.strSortBy = "Supplier" Then
                            '    foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, name ASC, Name ASC")
                            'Else
                            '    'foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, " & G_ReportOptions.strSortBy & " ASC, Name ASC")
                            '    foundRows = dtShoppingList.Select("Group = ''", "[Group] ASC, Name ASC")
                            'End If
                            intSwitch = 0
                            strGroup = Nothing
                        End If

                        For intRow As Integer = 0 To foundRows.GetUpperBound(0)
                            'Console.WriteLine(foundRows(intRow)("Group"))
                            'Console.WriteLine(foundRows(intRow)("Name"))

                            intCurrentX = 0
                            dblPrice = 0
                            dblUnitfactor = 0
                            dblQty = 0
                            dblGross = 0
                            dblMetUnitfactor = 0 'JTOC 09.09.2013
                            dblImpUnitfactor = 0 'JTOC 09.09.2013
                            dblMetQty = 0 'JTOC 09.09.2013
                            dblMetGross = 0 'JTOC 09.09.2013
                            dblImpQty = 0 'JTOC 09.09.2013
                            dblImpGross = 0 'JTOC 09.09.2013

                            strPriceFormat = CType(foundRows(intRow)("curFormat"), String)
                            sgType = CType(foundRows(intRow)("Type"), Single)
                            strName = CType(foundRows(intRow)("Name"), String)
                            strCurrency = ""
                            If Not IsDBNull(drvReport("Currency")) Then
                                strCurrency = CType(drvReport("Currency"), String)
                            End If
                            strNumber = ""
                            If Not IsDBNull(foundRows(intRow)("Number")) Then strNumber = CType(foundRows(intRow)("Number"), String)
                            strNumber = fctNumber2Text(strNumber)

                            dblPrice = 0
                            If Not IsDBNull(foundRows(intRow)("Price")) Then dblPrice = CType(foundRows(intRow)("Price"), Double)
                            dblPrice = Format(dblPrice, strPriceFormat)

                            If sgType = ID_Merchandise Then
                                dblUnitfactor = 0
                                If Not IsDBNull(foundRows(intRow)("UnitFactor")) Then dblUnitfactor = CType(foundRows(intRow)("UnitFactor"), Double)
                                strUnitFormat = foundRows(intRow)("UnitFormat").ToString
                            Else
                                dblUnitfactor = 1
                                strUnitFormat = G_FormatTwoDecimal
                            End If

                            If Not IsDBNull(foundRows(intRow)("NetQty")) Then dblQty = CType(foundRows(intRow)("NetQty"), Double)
                            If Not IsDBNull(foundRows(intRow)("GrossQty")) Then dblGross = CType(foundRows(intRow)("GrossQty"), Double)

                            'If Not IsDBNull(foundRows(intRow)("MetricNetQty")) Then dblMetQty = CType(foundRows(intRow)("MetricNetQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            'If Not IsDBNull(foundRows(intRow)("MetricGrossQty")) Then dblMetGross = CType(foundRows(intRow)("MetricGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            'If Not IsDBNull(foundRows(intRow)("ImperialNetQty")) Then dblImpQty = CType(foundRows(intRow)("ImperialNetQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            'If Not IsDBNull(foundRows(intRow)("ImperialGrossQty")) Then dblImpGross = CType(foundRows(intRow)("ImperialGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015

                            dblfactor = 1
                            If Not IsDBNull(foundRows(intRow)("PriceUnitFactor")) Then dblfactor = CType(foundRows(intRow)("PriceUnitFactor"), Double)

                            dblAmount = dblPrice * (dblGross * dblUnitfactor / dblfactor)
                            If Not IsDBNull(foundRows(intRow)("Amount")) Then dblAmount = CType(foundRows(intRow)("Amount"), Double)

                            'JTOC 09.09.2013
                            dblMetImpAmount = IIf(dblMetGross = 0, dblPrice * (dblImpGross * dblImpUnitfactor / dblfactor), dblPrice * (dblMetGross * dblMetUnitfactor / dblfactor))
                            'If Not IsDBNull(foundRows(intRow)("MetImpItemCost")) Then dblMetImpAmount = CType(foundRows(intRow)("MetImpItemCost"), Double) 'anm 12-28-2015


                            If sgType = ID_Merchandise Then
                                strUnitName = Trim(foundRows(intRow)("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(foundRows(intRow)("PriceUnit")) Then strPriceUnit = foundRows(intRow)("PriceUnit").ToString

                                'strMetUnitName = Trim(foundRows(intRow)("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                'strImpUnitName = Trim(foundRows(intRow)("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                            Else
                                strUnitName = Trim(foundRows(intRow)("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(foundRows(intRow)("Unit")) Then strPriceUnit = foundRows(intRow)("Unit").ToString

                                ' strMetUnitName = Trim(foundRows(intRow)("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                ' strImpUnitName = Trim(foundRows(intRow)("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                            End If

                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            sgFontSizeTemp = sgFontSize

                            If intSwitch = 1 Then
                                If UCase(strGroup) <> UCase(CType(foundRows(intRow)("Group"), String)) Then
                                    If Not IsNothing(strGroup) Then
                                        intCurrentY += intTextHeight
                                    End If

                                    strGroup = CType(foundRows(intRow)("Group"), String)
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroup, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                                    intCurrentY += intTextHeight + 10
                                    strGroupHeader = Nothing
                                End If

                                '--------------------------------------------------------
                                blnNewGroup = False
                                If G_ReportOptions.strSortBy = "CategoryName" Then
                                    If G_ReportOptions.strGroupBy <> "CategoryName" Then
                                        strX = CType(foundRows(intRow)("CategoryName"), String)
                                        If UCase(strGroupHeader) <> UCase(strX) Then
                                            strGroupHeader = strX
                                            strX = clang.GetString(clsEGSLanguage.CodeType.Category)
                                            blnNewGroup = True
                                        End If
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "Supplier" Then
                                    If G_ReportOptions.strGroupBy <> "Supplier" Then

                                        'Per Supplier
                                        strX = CType(foundRows(intRow)("Supplier"), String)
                                        If UCase(strGroupHeader) <> UCase(strX) Then
                                            strGroupHeader = strX
                                            strX = clang.GetString(clsEGSLanguage.CodeType.Supplier)
                                            blnNewGroup = True
                                        End If
                                    End If

                                ElseIf G_ReportOptions.strSortBy = "Price" And Not blnIncludePrice Then
                                    strX = strCurrency & " " & Format(dblPrice, strPriceFormat) & "/" & strPriceUnit
                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Price)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "GrossQty" And Not blnIncludeGrossQty Then
                                    'JTOC 09.09.2013
                                    If G_ReportOptions.blnUseMetricImperial Then
                                        strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    Else
                                        strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    End If

                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Gross_Qty)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "NetQty" And Not blnIncludeNetQty Then
                                    'JTOC 09.09.2013
                                    If G_ReportOptions.blnUseMetricImperial Then
                                        strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpQty, dblMetQty), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    Else
                                        strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                    End If

                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Net_Qty)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "Amount" And Not blnIncludePrice Then
                                    'JTOC 09.09.2013
                                    If G_ReportOptions.blnUseMetricImperial Then
                                        strX = strCurrency & " " & Format(dblMetImpAmount, strPriceFormat)
                                    Else
                                        strX = strCurrency & " " & Format(dblAmount, strPriceFormat)
                                    End If

                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Amount)
                                        blnNewGroup = True
                                    End If
                                ElseIf G_ReportOptions.strSortBy = "Number" And Not blnIncludeNumber Then
                                    strX = strNumber
                                    If UCase(strGroupHeader) <> UCase(strX) Then
                                        strGroupHeader = strX
                                        strX = clang.GetString(clsEGSLanguage.CodeType.Number)
                                        blnNewGroup = True
                                    End If
                                End If


                                If blnNewGroup Then
                                    intCurrentY += 5
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & ": " & strGroupHeader, fntItalic, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                    intCurrentY += intTextHeight + 5
                                End If
                                '------------------------------------------------------

                            Else
                                If strGroup = Nothing Then
                                    intCurrentY += intTextHeight
                                    strGroup = "***No Group***"
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroup, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                                    intCurrentY += intTextHeight + 10
                                End If
                            End If

                            If blnIncludeNumber Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strNumber, fntRegular, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(8)
                            End If

                            intTextHeightTemp = CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                            intTextHeight = intTextHeightTemp
                            If G_ReportOptions.blShrinkToFit Then
                                Do While Not intMaxHeight >= CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                                    sgFontSizeTemp -= 1
                                    fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                                Loop
                                intTextHeight = intMaxHeight
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            Else
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                            End If

                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            intCurrentX += L_lngNameW

                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(7)

                                strX = Format(dblPrice, strPriceFormat)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(1) - intColumnSpace

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel("/" & strPriceUnit, fntRegular, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(2) + intColumnSpace
                            End If

                            If blnIncludeGrossQty Then
                                strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(3)
                            End If

                            If blnIncludeNetQty Then
                                strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(4)
                            End If

                            If blnIncludeGrossQty Or blnIncludeNetQty Then

                                'JTOC 09.10.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = IIf(UCase((LTrim(strMetUnitName))) = "N/A", strImpUnitName, strMetUnitName)
                                Else
                                    strX = strUnitName
                                End If

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(5)
                            End If

                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(7)

                                strX = Format(dblAmount, strPriceFormat)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                            End If

                            'intCurrentY += intTextHeight 
                            intCurrentY += GetLineSpace(intMaxHeight)
                        Next


                        If intSwitch = 1 Then
                            GoTo Repeat
                        End If

                    Else        'Without Grouping

                        For Each drvReport In dtShoppingList.DefaultView
                            intCurrentX = 0
                            dblPrice = 0
                            dblUnitfactor = 0
                            dblQty = 0
                            dblGross = 0
                            dblMetUnitfactor = 0 'JTOC 09.09.2013
                            dblImpUnitfactor = 0 'JTOC 09.09.2013
                            dblMetQty = 0 'JTOC 09.09.2013
                            dblMetGross = 0 'JTOC 09.09.2013
                            dblImpQty = 0 'JTOC 09.09.2013
                            dblImpGross = 0 'JTOC 09.09.2013

                            strPriceFormat = CType(drvReport("curFormat"), String)
                            sgType = CType(drvReport("Type"), Single)
                            strName = drvReport("Name").ToString
                            strCurrency = drvReport("Currency").ToString
                            strNumber = ""
                            If Not IsDBNull(drvReport("Number")) Then strNumber = CType(drvReport("Number"), String)
                            strNumber = fctNumber2Text(strNumber)

                            dblPrice = 0
                            If Not IsDBNull(drvReport("Price")) Then dblPrice = CType(drvReport("Price"), Double)
                            dblPrice = Format(dblPrice, strPriceFormat)

                            If sgType = ID_Merchandise Then
                                dblUnitfactor = 0
                                If Not IsDBNull(drvReport("UnitFactor")) Then dblUnitfactor = CType(drvReport("UnitFactor"), Double)
                                strUnitFormat = drvReport("unitformat").ToString
                            Else
                                dblUnitfactor = 1
                                strUnitFormat = G_FormatTwoDecimal
                            End If

                            If Not IsDBNull(drvReport("NetQty")) Then dblQty = CType(drvReport("NetQty"), Double)
                            If Not IsDBNull(drvReport("GrossQty")) Then dblGross = CType(drvReport("GrossQty"), Double)

                            'If Not IsDBNull(drvReport("MetricNetQty")) Then dblMetQty = CType(drvReport("MetricNetQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            'If Not IsDBNull(drvReport("MetricGrossQty")) Then dblMetGross = CType(drvReport("MetricGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015
                            'If Not IsDBNull(drvReport("ImperialNetQty")) Then dblImpQty = CType(drvReport("ImperialNetQty"), Double) 'JTOC 09.09.2013  'anm 12-28-2015
                            'If Not IsDBNull(drvReport("ImperialGrossQty")) Then dblImpGross = CType(drvReport("ImperialGrossQty"), Double) 'JTOC 09.09.2013 'anm 12-28-2015

                            dblfactor = 1
                            If Not IsDBNull(drvReport("PriceUnitFactor")) Then dblfactor = CType(drvReport("PriceUnitFactor"), Double)

                            dblAmount = dblPrice * (dblGross * dblUnitfactor / dblfactor)
                            If Not IsDBNull(drvReport("Amount")) Then dblAmount = CType(drvReport("Amount"), Double)

                            'JTOC 09.09.2013
                            dblMetImpAmount = IIf(dblMetGross = 0, dblPrice * (dblImpGross * dblImpUnitfactor / dblfactor), dblPrice * (dblMetGross * dblMetUnitfactor / dblfactor))
                            ' If Not IsDBNull(drvReport("MetImpItemCost")) Then dblMetImpAmount = CType(drvReport("MetImpItemCost"), Double)'anm 12-28-2015

                            If sgType = ID_Merchandise Then
                                strUnitName = Trim(drvReport("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(drvReport("PriceUnit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                                'strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                'strImpUnitName = Trim(drvReport("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015

                            Else
                                strUnitName = Trim(drvReport("Unit").ToString)
                                strPriceUnit = ""
                                If Not IsDBNull(drvReport("Unit")) Then strPriceUnit = drvReport("PriceUnit").ToString

                                'strMetUnitName = Trim(drvReport("MetricUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015
                                'strImpUnitName = Trim(drvReport("ImperialUnit").ToString) 'JTOC 09.09.2013 'anm 12-28-2015

                            End If



                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            sgFontSizeTemp = sgFontSize
                            blnNewGroup = False
                            If G_ReportOptions.strSortBy = "CategoryName" Then
                                strX = CType(drvReport("CategoryName"), String)
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Supplier" Then
                                'Per Supplier
                                strX = CType(drvReport("Supplier"), String)
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Price" And Not blnIncludePrice Then
                                strX = strCurrency & " " & Format(dblPrice, strPriceFormat) & "/" & strPriceUnit
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "GrossQty" And Not blnIncludeGrossQty Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                Else
                                    strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                End If

                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "NetQty" And Not blnIncludeNetQty Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpQty, dblMetQty), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                Else
                                    strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                End If
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Amount" And Not blnIncludePrice Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = strCurrency & " " & Format(dblMetImpAmount, strPriceFormat)
                                Else
                                    strX = strCurrency & " " & Format(dblAmount, strPriceFormat)
                                End If

                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            ElseIf G_ReportOptions.strSortBy = "Number" And Not blnIncludeNumber Then
                                strX = strNumber
                                If UCase(strGroupHeader) <> UCase(strX) Then
                                    strGroupHeader = strX
                                    blnNewGroup = True
                                End If
                            End If


                            If blnNewGroup Then
                                intCurrentY += 5
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroupHeader, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                                intCurrentY += intTextHeight + 5
                            End If

                            If blnIncludeNumber Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strNumber, fntRegular, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(8)
                            End If

                            If G_ReportOptions.blShrinkToFit Then
                                intTextHeightTemp = CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                                Do While Not intMaxHeight >= CInt(ReportingTextUtils.MeasureText(strName, fntRegular, L_lngNameW, sf, Me.Padding).Height)
                                    sgFontSizeTemp -= 1
                                    fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                                Loop
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            Else
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strName, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                            End If


                            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            intCurrentX += L_lngNameW

                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(7)

                                strX = Format(dblPrice, strPriceFormat)
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(1) - intColumnSpace

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel("/" & strPriceUnit, fntRegular, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(2) + intColumnSpace
                            End If

                            If blnIncludeGrossQty Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpGross, dblMetGross), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                Else
                                    strX = fctFormatNumericQuantity(dblGross, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                End If

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(3)
                            End If

                            If blnIncludeNetQty Then
                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = fctFormatNumericQuantity(IIf(UCase((LTrim(strMetUnitName))) = "N/A", dblImpQty, dblMetQty), strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                Else
                                    strX = fctFormatNumericQuantity(dblQty, strUnitFormat, G_Options.RemoveTrailingZero, G_Options.QtyType)
                                End If

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(4)
                            End If

                            If blnIncludeGrossQty Or blnIncludeNetQty Then

                                'JTOC 09.10.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = IIf(UCase((LTrim(strMetUnitName))) = "N/A", strImpUnitName, strMetUnitName)
                                Else
                                    strX = strUnitName
                                End If

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                intCurrentX += L_lngCol(5)
                            End If

                            If blnIncludePrice Then
                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCurrency, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                                intCurrentX += L_lngCol(7)

                                'JTOC 09.09.2013
                                If G_ReportOptions.blnUseMetricImperial Then
                                    strX = Format(dblMetImpAmount, strPriceFormat)
                                Else
                                    strX = Format(dblAmount, strPriceFormat)
                                End If

                                .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                            End If
                            'intCurrentY += intTextHeight
                            intCurrentY += GetLineSpace(intMaxHeight)

                        Next
                    End If
                End With
            End If
            Cursor.Current = Cursors.Arrow
            dtShoppingList.Reset() 'VRP 11.03.2008
            Return Me
        Catch ex As Exception
            Dim x As String = ex.Message()
        End Try
    End Function

    '7 December 2005
    Function fctPrintNutrientValuesList(ByVal dtNutrientList As DataTable, _
                                        ByVal intType As Integer, ByVal intNutrientQty As Integer, _
                                        ByVal strFontName As String, ByVal sgFontSize As Single, _
                                        ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, _
                                        ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double, _
                                        ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, _
                                        ByVal udtUser As structUser, _
                                        Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16) As XtraReport 'VRP 05.11.2007

        Dim cLang As New clsEGSLanguage(udtUser.CodeLang)

        Dim drvReport As DataRowView
        Dim strNA As String
        Dim KiloCode As Long
        Dim LiterCode As Long
        Dim strReportTitle As String = Nothing
        Dim strSubHeading As String
        Dim intTextWidth As Integer
        Dim strNutField As String
        Dim strNutrientValues(0 To 16) As String
        Dim intMaxHeight As Integer
        Dim intMaxColumnWidth As Integer
        Dim introw As Integer
        Dim intLongestWidth As Integer

        Dim i As Integer
        ' Try
        strNA = cLang.GetString(clsEGSLanguage.CodeType.NA)  'N/A 

        fctGetUnitDetails(udtUser)
        KiloCode = fctGetUnitCodeFromType(100)
        LiterCode = fctGetUnitCodeFromType(200)
        L_NutrientCount = 16

        sf = New StringFormat(StringFormatFlags.NoClip)
        Try
            'If G_ReportOptions.blnPictureOneRight Then
            '    fntReportTitle = New Font(strFontName, 14, FontStyle.Bold)
            'Else
            '    fntReportTitle = New Font(strFontName, 16, FontStyle.Bold)
            'End If

            fntReportTitle = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'VRP 05.11.2007
            fntRegular = New Font(strFontName, 8, FontStyle.Regular)
            fntBold = New Font(strFontName, 8, FontStyle.Bold)
            fntFooter = New Font(strFontName, 8, FontStyle.Bold)
        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try

        xrLblEGS.Font = fntFooter
        If dtNutrientList.DefaultView.Count > 0 Then
            With Me
                Me.DataMember = dtNutrientList.TableName.ToString
                Me.DataSource = dtNutrientList

                'Papersize
                '------------
                .PaperKind = Printing.PaperKind.Custom
                .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

                'Margins
                '------------
                .Margins.Left = CInt(dblLeftMargin)
                .Margins.Top = CInt(dblTopMargin)
                .Margins.Bottom = CInt(dblBottomMargin)
                .Margins.Right = CInt(dblRightMargin)

                'Orientation
                '-----------
                .Landscape = True
                intAvailableWidth = .PageWidth

                intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin)

                .xrLinePF.Left = 0
                .xrLinePF.Width = intAvailableWidth
                .xrPIPageNumber.Left = intAvailableWidth / 2
                .xrPIPageNumber.Width = intAvailableWidth / 2
                .xrPIPageNumber.Font = fntBold
                .xrLblEGS.Left = 0
                .xrLblEGS.Width = intAvailableWidth / 2
                .xrLblEGS.Font = fntBold
                .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"


                If NoPrintLines Then
                    .xrLinePF.Visible = False
                End If
                subReportFooter(intAvailableWidth, strFontName)

                'fctGetNutrientDetails()
                L_NutrientCount = fctGetNutrientDetails(udtUser.CodeTrans, udtUser.Site.Code) + 1 'VRP 08.01.2009

                sf = New StringFormat(StringFormatFlags.NoClip)
                intTextHeight = ReportingTextUtils.MeasureText("A", fntBold, 100, sf, Me.Padding).Height

                intMaxColumnWidth = 52

                ReDim L_lngCol(17)
                sf = New StringFormat(StringFormatFlags.DirectionVertical)
                L_lngCol(1) = ReportingTextUtils.MeasureText(G_Nutrient(1).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(1) Then
                    If intLongestWidth < L_lngCol(1) Then intLongestWidth = L_lngCol(1)
                    L_lngCol(1) = intMaxColumnWidth
                End If

                'intMaxColumnWidth = L_lngCol(1)

                L_lngCol(2) = ReportingTextUtils.MeasureText(G_Nutrient(1).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(2) Then
                    If intLongestWidth < L_lngCol(2) Then intLongestWidth = L_lngCol(2)
                    L_lngCol(2) = intMaxColumnWidth
                End If

                L_lngCol(3) = ReportingTextUtils.MeasureText(G_Nutrient(2).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(3) Then
                    If intLongestWidth < L_lngCol(3) Then intLongestWidth = L_lngCol(3)
                    L_lngCol(3) = intMaxColumnWidth
                End If

                L_lngCol(4) = ReportingTextUtils.MeasureText(G_Nutrient(3).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(4) Then
                    If intLongestWidth < L_lngCol(4) Then intLongestWidth = L_lngCol(4)
                    L_lngCol(4) = intMaxColumnWidth
                End If

                L_lngCol(5) = ReportingTextUtils.MeasureText(G_Nutrient(4).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(5) Then
                    If intLongestWidth < L_lngCol(5) Then intLongestWidth = L_lngCol(5)
                    L_lngCol(5) = intMaxColumnWidth
                End If

                L_lngCol(6) = ReportingTextUtils.MeasureText(G_Nutrient(5).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(6) Then
                    If intLongestWidth < L_lngCol(6) Then intLongestWidth = L_lngCol(6)
                    L_lngCol(6) = intMaxColumnWidth
                End If

                L_lngCol(7) = ReportingTextUtils.MeasureText(G_Nutrient(6).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(7) Then
                    If intLongestWidth < L_lngCol(7) Then intLongestWidth = L_lngCol(7)
                    L_lngCol(7) = intMaxColumnWidth
                End If

                L_lngCol(8) = ReportingTextUtils.MeasureText(G_Nutrient(7).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(8) Then
                    If intLongestWidth < L_lngCol(8) Then intLongestWidth = L_lngCol(8)
                    L_lngCol(8) = intMaxColumnWidth
                End If

                L_lngCol(9) = ReportingTextUtils.MeasureText(G_Nutrient(8).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(9) Then
                    If intLongestWidth < L_lngCol(9) Then intLongestWidth = L_lngCol(9)
                    L_lngCol(9) = intMaxColumnWidth
                End If

                L_lngCol(10) = ReportingTextUtils.MeasureText(G_Nutrient(9).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(10) Then
                    If intLongestWidth < L_lngCol(10) Then intLongestWidth = L_lngCol(10)
                    L_lngCol(10) = intMaxColumnWidth
                End If

                L_lngCol(11) = ReportingTextUtils.MeasureText(G_Nutrient(10).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(11) Then
                    If intLongestWidth < L_lngCol(11) Then intLongestWidth = L_lngCol(11)
                    L_lngCol(11) = intMaxColumnWidth
                End If

                L_lngCol(12) = ReportingTextUtils.MeasureText(G_Nutrient(11).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(12) Then
                    If intLongestWidth < L_lngCol(12) Then intLongestWidth = L_lngCol(12)
                    L_lngCol(12) = intMaxColumnWidth
                End If

                L_lngCol(13) = ReportingTextUtils.MeasureText(G_Nutrient(12).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(13) Then
                    If intLongestWidth < L_lngCol(13) Then intLongestWidth = L_lngCol(13)
                    L_lngCol(13) = intMaxColumnWidth
                End If

                L_lngCol(14) = ReportingTextUtils.MeasureText(G_Nutrient(13).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(14) Then
                    If intLongestWidth < L_lngCol(14) Then intLongestWidth = L_lngCol(14)
                    L_lngCol(14) = intMaxColumnWidth
                End If

                L_lngCol(15) = ReportingTextUtils.MeasureText(G_Nutrient(14).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(15) Then
                    If intLongestWidth < L_lngCol(15) Then intLongestWidth = L_lngCol(15)
                    L_lngCol(15) = intMaxColumnWidth
                End If

                L_lngCol(16) = ReportingTextUtils.MeasureText(G_Nutrient(15).Name, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If intMaxColumnWidth < L_lngCol(16) Then
                    If intLongestWidth < L_lngCol(16) Then intLongestWidth = L_lngCol(16)
                    L_lngCol(16) = intMaxColumnWidth
                End If

                For Each drvReport In dtNutrientList.DefaultView
                    i = 1
                    For f As Integer = 1 To 34 '15
                        strNutField = "N" & f

                        If Not IsDBNull(drvReport(strNutField)) Then
                            If drvReport(strNutField).ToString <> "1.#INF" Then
                                strNutrientValues(i) = fctStrNutrientValue(CType(drvReport(strNutField), Double), G_Nutrient(f).Format, intNutrientQty, dblNutrientFactor, strNA)
                                intTextWidth = ReportingTextUtils.MeasureText(strNutrientValues(i), fntRegular, 0, sf, Padding).Width
                                L_lngCol(i) = L_lngCol(i) - intColumnSpace
                                If intTextWidth > L_lngCol(i) Then L_lngCol(i) = intTextWidth + intColumnSpace Else L_lngCol(i) = L_lngCol(i) + intColumnSpace
                            End If


                            'If f = 1 Then
                            '    i += 1
                            '    If drvReport(strNutField).ToString <> "1.#INF" Then
                            '        strNutrientValues(i) = fctStrNutrientValue(CType(drvReport(strNutField), Double) / ENERGYFACTOR, G_Nutrient(f).FormatKCAL, intNutrientQty, dblNutrientFactor, strNA)
                            '        intTextWidth = ReportingTextUtils.MeasureText(strNutrientValues(i), fntRegular, 0, sf, Padding).Width
                            '        If intTextWidth > L_lngCol(i) Then L_lngCol(i) = intTextWidth
                            '    End If
                            'End If
                        End If
                        i += 1
                    Next
                Next

                introw = intLongestWidth / intMaxColumnWidth

                For Each drvReport In dtNutrientList.DefaultView
                    Select Case intType
                        Case ID_Merchandise
                            dblNutrientFactor = 1

                        Case ID_Recipe
                            dblNutrientFactor = -1
                            dblNutrientFactor = fctGetRecipeNutrientFactor(False, CType(drvReport("YieldUnit"), Long), CType(drvReport("SrUnit"), Long), CType(drvReport("SrQty"), Double), CType(drvReport("srLevel"), Long), KiloCode, LiterCode)

                        Case ID_Menu
                            dblNutrientFactor = -1
                            If Not IsDBNull(drvReport("SrWeight")) Then dblNutrientFactor = fctGetMenuNutrientFactor(False, CType(drvReport("SrWeight"), Double))
                    End Select

                    i = 1
                    fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)

                    For f As Integer = 1 To 34 '15
                        strNutField = "N" & f
                        strX = ""
                        If Not IsDBNull(drvReport(strNutField)) Then
                            If drvReport(strNutField).ToString <> "1.#INF" Then strX = fctStrNutrientValue(CType(drvReport(strNutField), Double), G_Nutrient(1).Format, intNutrientQty, dblNutrientFactor, strNA)
                            intTextWidth = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                            If intTextWidth >= L_lngCol(i) Then L_lngCol(i) = intTextWidth
                        End If
                        i += 1

                        If f = 1 Then
                            strNutField = "N" & f
                            strX = ""
                            If Not IsDBNull(drvReport(strNutField)) Then
                                If drvReport(strNutField).ToString <> "1.#INF" Then strX = fctStrNutrientValue(CType(drvReport(strNutField), Double), G_Nutrient(1).Format, intNutrientQty, dblNutrientFactor, strNA)
                                intTextWidth = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                                If intTextWidth >= L_lngCol(i) Then L_lngCol(i) = intTextWidth
                                intMaxColumnWidth = intTextWidth
                            End If
                            i += 1
                        End If
                    Next

                    strX = ""
                    If Not IsDBNull(drvReport("YieldName")) Then
                        strX = Trim(drvReport("YieldName"))
                        intTextWidth = ReportingTextUtils.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        If intTextWidth >= L_lngCol(17) Then L_lngCol(17) = intTextWidth
                    End If
                Next

                L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1))

                For j As Integer = 2 To L_NutrientCount
                    L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(j))
                Next

                If intType = ID_Recipe Then L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(17))

                strSubHeading = ""
                Select Case intType
                    Case ID_Merchandise
                        strSubHeading = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100MG)  'Nutrient values are per 100g or 100ml 
                    Case ID_Recipe
                        Select Case intNutrientQty
                            Case 0 : strSubHeading = cLang.GetString(clsEGSLanguage.CodeType.Nutrient1Yield)  'Nutrient values are per 1 yield unit at 100%     
                            Case 1 : strSubHeading = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100G)  'Nutrient values are per 100g or 100ml at 100%
                            Case 2 : strSubHeading = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100ml)  'Nutrient values are per 1 yield unit/100g or 100 ml at 100%"  
                        End Select
                    Case ID_Menu
                        strSubHeading = cLang.GetString(clsEGSLanguage.CodeType.NutrientIsPerServingAt100)  'Nutrient values are per serving at 100%   
                End Select

                strSubHeading = "(" & strSubHeading & ")"

                Select Case intType
                    Case ID_Merchandise : strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.ProductNutrientList)  'Merchandise-Nutrient List 
                    Case ID_Recipe : strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.RecipeNutrientList)  'Recipe-Nutrient List 
                    Case ID_Menu : strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.MenuNutrientList)  'Menu-Nutrient List
                End Select

                intCurrentX = 0
                intCurrentY = 0

                'fntRegular = New Font("Arial Narrow", 8, FontStyle.Regular)

                'Report Title
                sf = New StringFormat(StringFormatFlags.NoClip)
                intTextHeight = ReportingTextUtils.MeasureText(strReportTitle, fntReportTitle, intAvailableWidth, sf, Me.Padding).Height
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strReportTitle, fntReportTitle, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})

                intCurrentY = intTextHeight
                'Sub Report Header
                intTextHeight = ReportingTextUtils.MeasureText(strSubHeading, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strSubHeading, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                intCurrentY += intTextHeight + 20


                .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
                intCurrentY += 10

                strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)  'Recipe
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})

                If intType = ID_Recipe Then
                    intCurrentX += L_lngNameW + L_lngCol(17)
                Else
                    intCurrentX += L_lngNameW
                End If

                Dim intTextHeightNut As Integer = intTextHeight

                sf = New StringFormat(StringFormatFlags.NoClip)
                For j As Integer = 1 To L_NutrientCount
                    If j > 2 Then
                        strX = G_Nutrient(j - 1).Name & vbCrLf & G_Nutrient(j - 1).Unit
                    Else
                        If j = 1 Then
                            strX = G_Nutrient(1).Name & vbCrLf & G_Nutrient(1).Unit
                        Else
                            strX = G_Nutrient(1).Name & vbCrLf & "kcal"
                        End If
                    End If
                    intTextHeightNut = fctGetHighest(strX, intTextHeightNut, L_lngCol(j) - intColumnSpace, fntBold, sf)
                Next


                For j As Integer = 1 To L_NutrientCount
                    If j > 2 Then
                        strX = G_Nutrient(j - 1).Name & vbCrLf & " " & G_Nutrient(j - 1).Unit
                    Else
                        If j = 1 Then
                            strX = G_Nutrient(1).Name & vbCrLf & " " & G_Nutrient(1).Unit
                        Else
                            strX = G_Nutrient(1).Name & vbCrLf & " kcal"
                        End If
                    End If
                    .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(j) - intColumnSpace, intTextHeightNut, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    intCurrentX += L_lngCol(j)
                Next

                intCurrentX = 0
                intCurrentY += intTextHeightNut
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})


                Dim sgFontSizeTemp As Single
                Dim intTextHeightTemp As Integer

                intCurrentY = 10
                intMaxHeight = ReportingTextUtils.MeasureText("A", fntRegular, intAvailableWidth, sf, Me.Padding).Height
                sf = New StringFormat(StringFormatFlags.NoClip)

                For Each drvReport In dtNutrientList.DefaultView
                    fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
                    sgFontSizeTemp = sgFontSize
                    Select Case intType
                        Case ID_Merchandise
                            dblNutrientFactor = 1
                        Case ID_Recipe
                            dblNutrientFactor = -1
                            dblNutrientFactor = fctGetRecipeNutrientFactor(False, CType(drvReport("YieldUnit"), Long), CType(drvReport("SrUnit"), Long), CType(drvReport("SrQty"), Double), CType(drvReport("srLevel"), Long), KiloCode, LiterCode)
                        Case ID_Menu
                            dblNutrientFactor = -1
                            If Not IsDBNull(drvReport("SrWeight")) Then dblNutrientFactor = fctGetMenuNutrientFactor(False, CType(drvReport("SrWeight"), Double))
                    End Select


                    i = 1
                    For f As Integer = 1 To 34 '15
                        strNutField = "N" & f

                        If Not IsDBNull(drvReport(strNutField)) Then
                            If drvReport(strNutField).ToString <> "1.#INF" Then strNutrientValues(i) = fctStrNutrientValue(CType(drvReport(strNutField), Double), G_Nutrient(f).Format, intNutrientQty, dblNutrientFactor, strNA)

                            'If f = 1 Then
                            '    i += 1
                            '    If drvReport(strNutField).ToString <> "1.#INF" Then strNutrientValues(i) = fctStrNutrientValue(CType(drvReport(strNutField), Double) / ENERGYFACTOR, G_Nutrient(f).Format, intNutrientQty, dblNutrientFactor, strNA)
                            'End If
                        End If
                        i += 1
                    Next

                    intCurrentX = 0
                    strX = VB.Trim(CType(drvReport("Name"), String))

                    fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)

                    intTextHeightTemp = ReportingTextUtils.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height
                    intTextHeight = intTextHeightTemp
                    If G_ReportOptions.blShrinkToFit Then
                        Do While Not intMaxHeight >= CInt(ReportingTextUtils.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
                            sgFontSizeTemp -= 1
                            fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                        Loop
                        intTextHeight = intMaxHeight
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    Else
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    End If

                    intCurrentX += L_lngNameW

                    If intType = ID_Recipe Then
                        If Not IsDBNull(drvReport("YieldName")) Then
                            strX = Trim(CType(drvReport("YieldName"), String))
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(17) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            intCurrentX += L_lngCol(17)
                        End If
                    End If

                    For j As Integer = 1 To L_NutrientCount
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strNutrientValues(j), fntRegular, intCurrentX, intCurrentY, L_lngCol(j) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                        intCurrentX += L_lngCol(j)
                    Next

                    'intCurrentY += intTextHeight + 2 
                    intCurrentY += GetLineSpace(intTextHeight) + 2
                Next
                dtNutrientList.Reset() 'VRP 11.03.2008
            End With
        End If
        Return Me
        'Catch ex As Exception

        'End Try

    End Function

    '' -- JBB New Nutrient Print for Recipe 
    Function PrintRecipeNutrientList(ByVal dtNutrientList As DataTable, ByVal intListeType As Integer,
                                       ByVal intNutrientQty As Integer, ByVal strFontName As String,
                                       ByVal sgFontSize As Single, ByVal dblPageWidth As Double,
                                       ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double,
                                       ByVal dblRightMargin As Double, ByVal dblTopMargin As Double,
                                       ByVal dblBottomMargin As Double, ByVal udtUser As structUser,
                                       ByVal arrblNutrientDisplay() As Boolean,
                                       Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16,
                             Optional ByVal userLocale As String = "en-US") As XtraReport 'VRP 05.11.2007

        '--- PAGE PROPERTIES ---
        If Not dtNutrientList.DefaultView.Count > 0 Then Return Me
        Dim cLang As New clsEGSLanguage(udtUser.CodeLang)
        'Dim strimpose As String, strImposedPercent As String, strImposeddisplay As String ' JBB 05.24.2012
        sf = New StringFormat(StringFormatFlags.NoClip)
        Dim userCulture As CultureInfo = New CultureInfo(userLocale)
        ' Dim cLi As New clsLicense
        Try
            fntHeader1 = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold)
            fntHeader2 = New Font(strFontName, 8, FontStyle.Regular)
            fntDetail1 = New Font(strFontName, 8, FontStyle.Bold)
            fntDetail2 = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntFooter = New Font(strFontName, 8, FontStyle.Bold)
        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try

        With Me
            .PaperKind = Printing.PaperKind.Custom
            .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)
            .Margins.Left = CInt(dblLeftMargin)
            .Margins.Top = CInt(dblTopMargin)
            .Margins.Bottom = CInt(dblBottomMargin)
            .Margins.Right = CInt(dblRightMargin)

            .Landscape = True
            intAvailableWidth = .PageWidth
            intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin)
        End With
        '--- PAGE PROPERTIES ---

        '--- PAGE FOOTER ---
        With Me
            .xrLinePF.Left = 0
            .xrLinePF.Width = intAvailableWidth
            .xrPIPageNumber.Left = intAvailableWidth / 2
            .xrPIPageNumber.Width = intAvailableWidth / 2
            .xrPIPageNumber.Font = fntDetail1
            .xrLblEGS.Left = 0
            .xrLblEGS.Width = intAvailableWidth / 2
            .xrLblEGS.Font = fntDetail1
            .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

            If NoPrintLines Then
                .xrLinePF.Visible = False
            End If
            subReportFooter(intAvailableWidth, strFontName)
        End With
        '--- PAGE FOOTER ---

        '--- PAGE HEADER ---
        With Me
            'PRINT HEADER
            intCurrentX = 0
            intCurrentY = 0

            'Main Header
            Select Case intListeType
                Case ID_Merchandise : strX = cLang.GetString(clsEGSLanguage.CodeType.ProductNutrientList)
                Case ID_Recipe : strX = cLang.GetString(clsEGSLanguage.CodeType.RecipeNutrientList)
                Case ID_Menu : strX = cLang.GetString(clsEGSLanguage.CodeType.MenuNutrientList)
            End Select
            intTextWidth = ReportingTextUtils.MeasureText(strX, fntHeader1, 0, sf, Padding).Width
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntHeader1, intTextWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntHeader1, IIf(G_ReportOptions.strTitleColor = "red", Color.Red, Color.Black), Color.Transparent, intCurrentX, intCurrentY, intTextWidth + 300, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft)})
            intCurrentY += intTextHeight

            'Sub Header
            Select Case intListeType
                Case ID_Merchandise : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100MG)  'Nutrient values are per 100g or 100ml 
                Case ID_Recipe
                    Select Case intNutrientQty
                        Case 0 : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient1Yield)  'Nutrient values are per 1 yield unit at 100%     
                        Case 1 : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100G)  'Nutrient values are per 100g or 100ml at 100%
                        Case 2 : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100ml)  'Nutrient values are per 1 yield unit/100g or 100 ml at 100%"  
                    End Select
                Case ID_Menu : strX = cLang.GetString(clsEGSLanguage.CodeType.NutrientIsPerServingAt100)
            End Select
            strX = "(" & strX & ")"

            intTextWidth = ReportingTextUtils.MeasureText(strX, fntHeader2, 0, sf, Padding).Width
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntHeader2, intTextWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader2, intCurrentX, intCurrentY, intTextWidth + 300, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentY += intTextHeight + 20

            'Line
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
            intCurrentY += 10
        End With
        '--- PAGE HEADER ---

        '--- PAGE DETAIL ---
        With Me
            fctOpenDatabase(strCnn)
            L_NutrientCount = fctGetNutrientDetails(udtUser.CodeTrans, udtUser.Site.Code, strConnection:=G_strConnection)

            'MEASURE WIDTH
            Dim L_lngNutCol(35) As Long '  L_lngNutCol(15) 
            Dim L_lngNameCol As Long = 0
            Dim L_lngYieldCol As Long = 0
            Dim L_lngNutRow As Long = 0

            Dim KiloCode As Long = fctGetUnitCodeFromType(100)
            Dim LiterCode As Long = fctGetUnitCodeFromType(200)

            'Nutrient Header
            '-- JBB 08.20.2012
            For dn As Integer = 1 To L_NutrientCount
                For n As Integer = 1 To L_NutrientCount
                    If G_Nutrient(n).DisplayPosition = dn Then
                        If arrblNutrientDisplay(n) = True Then
                            strX = G_Nutrient(n).Name
                            L_lngNutCol(n) = ReportingTextUtils.MeasureText(strX, fntDetail1, 0, sf, Me.Padding).Width
                        End If
                        Exit For
                    End If
                Next
            Next
            '--
            'For n As Integer = 1 To L_NutrientCount
            '    If arrblNutrientDisplay(n) = True Then
            '        strX = G_Nutrient(n).Name
            '        L_lngNutCol(n) = ReportingTextUtils.MeasureText(strX, fntDetail1, 0, sf, Me.Padding).Width
            '    End If
            'Next
            '--
            'Nutrient Values
            Dim strNA As String = cLang.GetString(clsEGSLanguage.CodeType.NA)
            For Each drv As DataRowView In dtNutrientList.DefaultView
                'Get Factor
                Select Case intListeType
                    Case ID_Merchandise : dblNutrientFactor = 1
                    Case ID_Recipe 'dblNutrientFactor = fctGetRecipeNutrientFactor(False, CType(drv("YieldUnit"), Long), CType(drv("SrUnit"), Long), CType(drv("SrQty"), Double), CType(drv("srLevel"), Long), KiloCode, LiterCode)
                        dblNutrientFactor = GetNutrientFactor(CInt(drv("Code")))
                    Case ID_Menu
                        If Not IsDBNull(drv("SrWeight")) Then dblNutrientFactor = fctGetMenuNutrientFactor(False, CType(drv("SrWeight"), Double))
                End Select

                'Get Nutrient Values
                For n As Integer = 1 To L_NutrientCount
                    If arrblNutrientDisplay(n) = True Then
                        strX = "N" & n

                        ' ''-- JBB 05.24.2012
                        'strImposeddisplay = "N" & n & "Display"
                        'strImposedPercent = "N" & n & "ImposePercent"
                        'strimpose = "N" & n & "impose"
                        'Dim dblNIP As Double, dblNI As Double
                        'dblNI = IIf(IsDBNull(drv(strimpose)), -1, CDblDB(drv(strimpose)))
                        'dblNIP = IIf(IsDBNull(drv(strImposedPercent)), -1, CDblDB(drv(strImposedPercent)))
                        'If dblNIP = -1 Then
                        '    strX = strimpose
                        'Else
                        '    strX = strImposedPercent
                        'End If
                        ' ''--


                        'If Not IsDBNull(drv(strX)) Then
                        '    If drv(strX).ToString <> "1.#INF" Then
                        '        strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                        '    Else
                        '        strX = ""
                        '    End If
                        '    intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
                        '    If intTextWidth > L_lngNutCol(n) Then L_lngNutCol(n) = intTextWidth
                        'End If

                        'If n = 1 Then
                        '    ''strX = "N" & n ''-- JBB 05.24.2012
                        '    If dblNIP = -1 Then
                        '        strX = strimpose
                        '    Else
                        '        strX = strImposedPercent
                        '    End If
                        '    If Not IsDBNull(drv(strX)) Then
                        '        If drv(strX).ToString <> "1.#INF" Then
                        '            strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                        '        End If

                        '        intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
                        '        If intTextWidth > L_lngNutCol(n) Then L_lngNutCol(n) = intTextWidth
                        '    End If
                        'End If
                        ''-- JBB 05.24.2012
                        Dim dblNIP As Double = 0, dblNI As Double = 0
                        ' RDC 11.04.2013 :  Fixed error while printing recipe nutrient list
                        If Not IsDBNull(drv(strX)) Then dblNI = drv(strX)
                        'dblNI = drv(strX) 'IIf(, CDblDB(drv(strimpose)))
                        'dblNIP = IIf(IsDBNull(drv(strImposedPercent)), -1, CDblDB(drv(strImposedPercent)))
                        'If dblNIP = -1 Then
                        '    strX = strimpose
                        'Else
                        '    strX = strImposedPercent
                        'End If
                        ''--

                        If Not IsDBNull(drv(strX)) Then
                            If drv(strX).ToString <> "1.#INF" Then
                                'strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                Dim dblY As Double = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                If dblY = 0 Then
                                    strX = "0"
                                Else
                                    strX = dblY.ToString(G_Nutrient(n).Format, userCulture)
                                End If
                            Else
                                strX = ""
                                L_lngYieldCol = 0
                            End If
                            intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
                            If intTextWidth > L_lngNutCol(n) Then L_lngNutCol(n) = intTextWidth
                        Else
                            strX = "-"
                        End If

                        If n = 1 Then
                            strX = "N" & n ''-- JBB 05.24.2012
                            'If dblNIP = -1 Then
                            '    strX = strimpose
                            'Else
                            '    strX = strImposedPercent
                            'End If
                            If Not IsDBNull(drv(strX)) Then
                                If drv(strX).ToString <> "1.#INF" Then
                                    'strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                    Dim dblY As Double = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                    If dblY = 0 Then
                                        strX = "0"
                                    Else
                                        strX = dblY.ToString(G_Nutrient(n).Format, userCulture)
                                    End If
                                End If

                                intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
                                If intTextWidth > L_lngNutCol(n) Then L_lngNutCol(n) = intTextWidth
                            Else
                                strX = "-"
                            End If
                        End If
                    End If
                Next

                strX = ""
                If Not IsDBNull(drv("YieldName")) Then
                    strX = Trim(drv("YieldName"))
                    intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
                    If intTextWidth > L_lngYieldCol Then L_lngYieldCol = intTextWidth
                End If
            Next

            'Recipe Names
            L_lngNameCol = 0
            For n As Integer = 1 To L_NutrientCount
                If arrblNutrientDisplay(n) = True Then '-- JBB 05.24.2012
                    L_lngNutCol(n) += intColumnSpace
                    L_lngNameCol += L_lngNutCol(n)
                    If n = 1 Then L_lngNameCol += L_lngNutCol(n)
                End If
            Next
            L_lngYieldCol += intColumnSpace
            L_lngNameCol += L_lngYieldCol
            L_lngNameCol = (intAvailableWidth - L_lngNameCol)

            'AGL 2013.05.14
            If L_lngNameCol < 50 Then
                L_lngNameCol = 50
            End If

            'MEASURE HEIGHT - Nutrient Header
            For i As Integer = 1 To L_NutrientCount
                If arrblNutrientDisplay(i) = True Then '-- JBB 05.24.2012
                    strX = G_Nutrient(i).Name & " " & G_Nutrient(i).Unit
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngNutCol(i) - intColumnSpace, sf, Me.Padding).Height
                    If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight


                    If i = 1 Then 'kcal
                        strX = G_Nutrient(i).Name & " kcal"
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngNutCol(i) - intColumnSpace, sf, Me.Padding).Height
                        If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
                    End If
                End If
            Next

            'PRINT DETAIL - Nutrient Header
            intCurrentX = 0
            intCurrentY = 0

            If L_lngNutRow = 0 Then
                L_lngNutRow = 15
            End If
            strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)  'Recipe
            .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNameCol - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentX += L_lngNameCol + L_lngYieldCol

            '-- JBB 05.20.2012
            For dn As Integer = 1 To L_NutrientCount
                For n As Integer = 1 To L_NutrientCount
                    If G_Nutrient(n).DisplayPosition = dn Then
                        If arrblNutrientDisplay(n) = True Then '-- JBB 05.24.2012
                            strX = G_Nutrient(n).Name & vbCrLf & " " & G_Nutrient(n).Unit
                            .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            intCurrentX += L_lngNutCol(n)


                            'If n = 1 Then
                            '    'kcal
                            '    strX = G_Nutrient(1).Name  '  G_Nutrient(1).Name & vbCrLf & " kcal" '-- JBB 08.20.2012
                            '    .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                            '    intCurrentX += L_lngNutCol(n)
                            'End If
                        End If
                        Exit For
                    End If
                Next
            Next
            ''--
            'For n As Integer = 1 To L_NutrientCount
            '    If arrblNutrientDisplay(n) = True Then '-- JBB 05.24.2012
            '        strX = G_Nutrient(n).Name & vbCrLf & " " & G_Nutrient(n).Unit
            '        .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '        intCurrentX += L_lngNutCol(n)

            '        If n = 1 Then
            '            'kcal
            '            strX = G_Nutrient(1).Name  '  G_Nutrient(1).Name & vbCrLf & " kcal" '-- JBB 08.20.2012
            '            .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '            intCurrentX += L_lngNutCol(n)
            '        End If
            '    End If
            'Next
            ''--
            intCurrentX = 0
            intCurrentY += L_lngNutRow
            .Detail.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})
            intCurrentY += 10

            L_lngNutRow = 0
            For Each drv As DataRowView In dtNutrientList.DefaultView
                'Get Factor
                Select Case intListeType
                    Case ID_Merchandise : dblNutrientFactor = 1
                    Case ID_Recipe 'dblNutrientFactor = fctGetRecipeNutrientFactor(False, CType(drv("YieldUnit"), Long), CType(drv("SrUnit"), Long), CType(drv("SrQty"), Double), CType(drv("srLevel"), Long), KiloCode, LiterCode)
                        dblNutrientFactor = GetNutrientFactor(CInt(drv("Code")))
                    Case ID_Menu
                        If Not IsDBNull(drv("SrWeight")) Then dblNutrientFactor = fctGetMenuNutrientFactor(False, CType(drv("SrWeight"), Double))
                End Select

                'MEASURE HEIGHT - Nutrient Values
                For n As Integer = 1 To L_NutrientCount
                    If arrblNutrientDisplay(n) = True Then '-- JBB 05.24.2012
                        strX = "N" & n 'AGL 2013.05.17
                        ''-- JBB 05.24.2012
                        'strImposeddisplay = "N" & n & "Display"
                        'strImposedPercent = "N" & n & "ImposePercent"
                        'strimpose = "N" & n & "impose"
                        ' 
                        Dim dblNIP As Double = 0, dblNI As Double = 0
                        ' RDC 11.04.2013 :  Fixed error while printing recipe nutrient list
                        If Not IsDBNull(drv(strX)) Then dblNI = drv(strX)
                        'dblNI = drv(strX) 'IIf(, CDblDB(drv(strimpose)))
                        'dblNIP = IIf(IsDBNull(drv(strImposedPercent)), -1, CDblDB(drv(strImposedPercent)))
                        'If dblNIP = -1 Then
                        '    strX = strimpose
                        'Else
                        '    strX = strImposedPercent
                        'End If
                        '--

                        If Not IsDBNull(drv(strX)) Then
                            If drv(strX).ToString <> "1.#INF" Then
                                'strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                Dim dblY As Double = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                If dblY = 0 Then
                                    strX = "0"
                                Else
                                    strX = dblY.ToString(G_Nutrient(n).Format, userCulture)
                                End If
                            Else
                                strX = ""
                            End If
                            intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNutCol(n) - intColumnSpace, sf, Me.Padding).Height
                            If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
                        Else
                            strX = "-"
                        End If

                        If n = 1 Then
                            strX = "N" & n
                            If Not IsDBNull(drv(strX)) Then
                                If drv(strX).ToString <> "1.#INF" Then
                                    'strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                    Dim dblY As Double = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                    If dblY = 0 Then
                                        strX = "0"
                                    Else
                                        strX = dblY.ToString(G_Nutrient(n).Format, userCulture)
                                    End If
                                End If
                                intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNutCol(n) - intColumnSpace, sf, Me.Padding).Height
                                If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
                            Else
                                strX = "-"
                            End If
                        End If
                    End If
                Next

                strX = ""
                If Not IsDBNull(drv("YieldName")) Then
                    strX = Trim(drv("YieldName"))
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngYieldCol - intColumnSpace, sf, Me.Padding).Height
                    If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
                End If

                If Not IsDBNull(drv("Name")) Then
                    strX = VB.Trim(CType(drv("Name"), String))
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNameCol - intColumnSpace, sf, Me.Padding).Height
                    If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
                End If


                'PRINT DETAIL - Nutrient Values
                'Recipe Name
                If Not IsDBNull(drv("Name")) Then
                    strX = VB.Trim(CType(drv("Name"), String))
                    'AGL 2013.05.14
                    '.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNameCol - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNameCol, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})
                End If
                intCurrentX += L_lngNameCol

                'Yield 
                If Not IsDBNull(drv("YieldName")) Then
                    strX = VB.Trim(CType(drv("YieldName"), String))
                    'AGL 2013.05.14
                    '.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngYieldCol - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngYieldCol, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})
                End If
                intCurrentX += L_lngYieldCol

                'Nutrient
                '-- JBB 08.20.2012
                For dn As Integer = 1 To L_NutrientCount
                    For n As Integer = 1 To L_NutrientCount
                        If G_Nutrient(n).DisplayPosition = dn Then
                            If arrblNutrientDisplay(n) = True Then '-- JBB 05.24.2012
                                strX = "N" & n 'AGL 2013.05.17
                                ''-- JBB 05.24.2012
                                'strImposeddisplay = "N" & n & "Display"
                                'strImposedPercent = "N" & n & "ImposePercent"
                                'strimpose = "N" & n & "impose"
                                Dim dblNIP As Double = 0, dblNI As Double = 0
                                ' RDC 11.04.2013 :  Fixed error while printing recipe nutrient list
                                If Not IsDBNull(drv(strX)) Then dblNI = drv(strX)
                                'dblNI = drv(strX) 'IIf(, CDblDB(drv(strimpose)))
                                'dblNIP = IIf(IsDBNull(drv(strImposedPercent)), -1, CDblDB(drv(strImposedPercent)))
                                'If dblNIP = -1 Then
                                '    strX = strimpose
                                'Else
                                '    strX = strImposedPercent
                                'End If
                                '--
                                If Not IsDBNull(drv(strX)) Then
                                    If drv(strX).ToString <> "1.#INF" Then
                                        If drv(strX).ToString = "-1" Then
                                            strX = "-"
                                        Else
                                            If drv(strX).ToString = "0" Then
                                                strX = "0"
                                            Else
                                                Dim dblY As Double = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                                strX = dblY.ToString(G_Nutrient(n).Format, userCulture)
                                            End If
                                        End If
                                    Else
                                        strX = ""
                                    End If
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                Else
                                    strX = "-"
                                    .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                End If
                                intCurrentX += L_lngNutCol(n)

                                'If n = 1 Then
                                '    strX = "N" & n
                                '    If Not IsDBNull(drv(strX)) Then
                                '        If drv(strX).ToString <> "1.#INF" Then
                                '            strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                                '        End If
                                '        .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                                '    End If
                                '    intCurrentX += L_lngNutCol(n)
                                'End If
                            End If
                            Exit For
                        End If
                    Next
                Next
                ''--
                'For n As Integer = 1 To L_NutrientCount
                '    If arrblNutrientDisplay(n) = True Then '-- JBB 05.24.2012
                '        'strX = "N" & n
                '        ''-- JBB 05.24.2012
                '        strImposeddisplay = "N" & n & "Display"
                '        strImposedPercent = "N" & n & "ImposePercent"
                '        strimpose = "N" & n & "impose"
                '        Dim dblNIP As Double, dblNI As Double
                '        dblNI = IIf(IsDBNull(drv(strimpose)), -1, CDblDB(drv(strimpose)))
                '        dblNIP = IIf(IsDBNull(drv(strImposedPercent)), -1, CDblDB(drv(strImposedPercent)))
                '        If dblNIP = -1 Then
                '            strX = strimpose
                '        Else
                '            strX = strImposedPercent
                '        End If
                '        '--
                '        If Not IsDBNull(drv(strX)) Then
                '            If drv(strX).ToString <> "1.#INF" Then
                '                strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                '            Else
                '                strX = ""
                '            End If
                '            .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                '        End If
                '        intCurrentX += L_lngNutCol(n)

                '        If n = 1 Then
                '            strX = "N" & n
                '            If Not IsDBNull(drv(strX)) Then
                '                If drv(strX).ToString <> "1.#INF" Then
                '                    strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
                '                End If
                '                .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                '            End If
                '            intCurrentX += L_lngNutCol(n)
                '        End If
                '    End If
                'Next
                ''--
                intCurrentY += L_lngNutRow
                intCurrentX = 0
            Next
        End With
        '--- PAGE DETAIL ---
        Return Me
    End Function




    'VRP 25.06.2009
    Function PrintNutrientList(ByVal dtNutrientList As DataTable, ByVal intListeType As Integer,
                                       ByVal intNutrientQty As Integer, ByVal strFontName As String,
                                       ByVal sgFontSize As Single, ByVal dblPageWidth As Double,
                                       ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double,
                                       ByVal dblRightMargin As Double, ByVal dblTopMargin As Double,
                                       ByVal dblBottomMargin As Double, ByVal udtUser As structUser,
                                       Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16,
                             Optional ByVal userLocale As String = "en-US") As XtraReport 'VRP 05.11.2007

        '--- PAGE PROPERTIES ---
        If Not dtNutrientList.DefaultView.Count > 0 Then Return Me
        Dim cLang As New clsEGSLanguage(udtUser.CodeLang)
        sf = New StringFormat(StringFormatFlags.NoClip)

        Dim userCulture As CultureInfo = New CultureInfo(userLocale)

        Try
            fntHeader1 = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold)
            fntHeader2 = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntDetail1 = New Font(strFontName, sgFontSize, FontStyle.Bold)
            fntDetail2 = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntFooter = New Font(strFontName, sgFontSize, FontStyle.Bold)
        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try

        With Me
            ' RDC 10.30.2013 : Changed PaperKind property from Custom to Legal
            '.PaperKind = Printing.PaperKind.Custom
            .PaperKind = Printing.PaperKind.Legal
            .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)
            .Margins.Left = CInt(dblLeftMargin)
            .Margins.Top = CInt(dblTopMargin)
            .Margins.Bottom = CInt(dblBottomMargin)
            .Margins.Right = CInt(dblRightMargin)

            .Landscape = True
            intAvailableWidth = .PageWidth
            intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin)
        End With
        '--- PAGE PROPERTIES ---

        '--- PAGE FOOTER ---
        With Me
            .xrLinePF.Left = 0
            .xrLinePF.Width = intAvailableWidth
            .xrPIPageNumber.Left = intAvailableWidth / 2
            .xrPIPageNumber.Width = intAvailableWidth / 2
            .xrPIPageNumber.Font = fntDetail1
            .xrLblEGS.Left = 0
            .xrLblEGS.Width = intAvailableWidth / 2
            .xrLblEGS.Font = fntDetail1
            .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

            If NoPrintLines Then
                .xrLinePF.Visible = False
            End If
            subReportFooter(intAvailableWidth, strFontName)
        End With
        '--- PAGE FOOTER ---

        '--- PAGE HEADER ---
        With Me
            'PRINT HEADER
            intCurrentX = 0
            intCurrentY = 0

            'Main Header
            Select Case intListeType
                Case ID_Merchandise : strX = cLang.GetString(clsEGSLanguage.CodeType.ProductNutrientList)
                Case ID_Recipe : strX = cLang.GetString(clsEGSLanguage.CodeType.RecipeNutrientList)
                Case ID_Menu : strX = cLang.GetString(clsEGSLanguage.CodeType.MenuNutrientList)
            End Select
            intTextWidth = ReportingTextUtils.MeasureText(strX, fntHeader1, 0, sf, Padding).Width
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntHeader1, intTextWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntHeader1, IIf(G_ReportOptions.strTitleColor = "red", Color.Red, Color.Black), Color.Transparent, intCurrentX, intCurrentY, intTextWidth + 300, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft)})
            intCurrentY += intTextHeight

            'Sub Header
            Select Case intListeType
                Case ID_Merchandise : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100MG)  'Nutrient values are per 100g or 100ml 
                Case ID_Recipe
                    Select Case intNutrientQty
                        Case 0 : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient1Yield)  'Nutrient values are per 1 yield unit at 100%     
                        Case 1 : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100G)  'Nutrient values are per 100g or 100ml at 100%
                        Case 2 : strX = cLang.GetString(clsEGSLanguage.CodeType.Nutrient100ml)  'Nutrient values are per 1 yield unit/100g or 100 ml at 100%"  
                    End Select
                Case ID_Menu : strX = cLang.GetString(clsEGSLanguage.CodeType.NutrientIsPerServingAt100)
            End Select
            strX = "(" & strX & ")"

            intTextWidth = ReportingTextUtils.MeasureText(strX, fntHeader2, 0, sf, Padding).Width
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntHeader2, intTextWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader2, intCurrentX, intCurrentY, intTextWidth + 300, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentY += intTextHeight + 20

            'AGL 2013.03.14 - moved to below
            'Line
            '.PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
            intCurrentY += 10
        End With
        '--- PAGE HEADER ---

        '--- PAGE DETAIL ---
        With Me
            intCurrentX = 0
            intCurrentY = 0

            L_NutrientCount = fctGetNutrientDetails(udtUser.CodeTrans, udtUser.Site.Code, 0, G_strConnection)

            Dim strItemHeader As String = ""
            Select Case intListeType
                Case ID_Merchandise
                    strItemHeader = cLang.GetString(clsEGSLanguage.CodeType.Product)
                Case ID_Recipe
                    strItemHeader = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
                Case Else

            End Select

            Dim xrTNutList, xrTHNutList As New XRTable
            Dim xrRNutList, xrRHNutList As New XRTableRow
            Dim xrCNutList, xrCHNutList As New XRTableCell
            Dim intHWidth As Integer = CInt(CInt(intAvailableWidth - 150) / (L_NutrientCount + 1))

            ' Report Detail Headers
            xrTHNutList.Location = New Point(intCurrentX, intCurrentY)
            xrTHNutList.Width = intAvailableWidth
            xrTHNutList.Borders = DevExpress.XtraPrinting.BorderSide.Top
            xrTHNutList.BorderWidth = 1
            xrRHNutList.Width = xrTHNutList.Width

            For intHeader As Integer = 1 To CInt(L_NutrientCount + 1) Step 1
                If intHeader = 1 Then
                    xrCHNutList = New XRTableCell
                    xrCHNutList.Width = CInt(150)
                    xrCHNutList.CanShrink = True
                    xrCHNutList.Text = strItemHeader
                    xrCHNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Bold)

                    xrCHNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
                    xrRHNutList.Cells.Add(xrCHNutList)
                ElseIf intHeader = 2 Then
                    For intEnergy As Integer = 1 To 2 Step 1
                        If intEnergy = 1 Then
                            xrCHNutList = New XRTableCell
                            xrCHNutList.Width = CInt(intHWidth)
                            xrCHNutList.CanShrink = True
                            xrCHNutList.Text = G_Nutrient(intHeader - 1).Name & " kj"
                            xrCHNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Bold)
                            xrCHNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
                            xrRHNutList.Cells.Add(xrCHNutList)
                        Else
                            xrCHNutList = New XRTableCell
                            xrCHNutList.Width = CInt(intHWidth)
                            xrCHNutList.CanShrink = True
                            xrCHNutList.Text = G_Nutrient(intHeader - 1).Name & " kcal"
                            xrCHNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Bold)
                            xrCHNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
                            xrRHNutList.Cells.Add(xrCHNutList)
                        End If

                    Next
                Else
                    xrCHNutList = New XRTableCell
                    xrCHNutList.Width = CInt(intHWidth)
                    xrCHNutList.CanShrink = True
                    xrCHNutList.Text = G_Nutrient(intHeader - 1).Name
                    xrCHNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Bold)
                    xrCHNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
                    xrRHNutList.Cells.Add(xrCHNutList)
                End If

            Next
            xrTHNutList.Rows.Add(xrRHNutList)
            xrTHNutList.Location = New Point(intCurrentX, intCurrentY) ' kmqdc
            .Detail.Controls.Add(xrTHNutList)
            intCurrentY += (xrTHNutList.Height) 'KMQDC 2016.08.11 original value is 1

            Dim xrHLine As New XRLine
            xrHLine.Location = New Point(intCurrentX, intCurrentY)
            xrHLine.BorderWidth = 5
            xrHLine.Width = intAvailableWidth
            .Detail.Controls.Add(xrHLine)
            intCurrentY += (xrHLine.Height + 5)


            ' Nutrient Details

            xrTNutList.Location = New Point(intCurrentX, intCurrentY)
            xrTNutList.Width = intAvailableWidth

            For Each drvNutList As DataRowView In dtNutrientList.DefaultView
                xrRNutList = New XRTableRow
                xrRHNutList.Width = xrTNutList.Width
                ' Get Nutrient Factor
                Select Case intListeType
                    Case ID_Merchandise
                        dblNutrientFactor = 1
                    Case ID_Recipe
                        dblNutrientFactor = GetNutrientFactor(CInt(drvNutList("Code")))
                    Case ID_Menu
                        If Not IsDBNull(drvNutList("SrWeight")) Then dblNutrientFactor = fctGetMenuNutrientFactor(False, CType(drvNutList("SrWeight"), Double))
                End Select
                ' Get Nutrient Values

                For intDetail As Integer = 1 To (L_NutrientCount + 1) Step 1
                    Dim strFieldName As String = "N" & (intDetail - 1).ToString
                    Select Case intDetail
                        Case 1      ' Item Name
                            Dim strItemName As String = ""
                            If IsDBNull(drvNutList("Name")) Then strItemName = "" Else strItemName = drvNutList("Name").ToString.Trim
                            xrCNutList = New XRTableCell
                            xrCNutList.CanShrink = False
                            xrCNutList.Width = CInt(150)
                            xrCNutList.Text = strItemName
                            xrCNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            xrCNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
                            xrRNutList.Cells.Add(xrCNutList)
                        Case 2      ' Energy kj/kcal
                            Dim strNutrientVal As String = ""
                            For iEnergy As Integer = 1 To 2 Step 1
                                Select Case iEnergy
                                    Case 1
                                        If Not IsDBNull(drvNutList(strFieldName)) And Not drvNutList(strFieldName).ToString = "1.#INF" And Not CDbl(drvNutList(strFieldName)) = -1 Then
                                            Dim dblY As Double = fctStrNutrientValue(Format(CType(drvNutList("N" & CStr(intDetail - 1)), Double), G_Nutrient(intDetail - 1).Format), G_Nutrient(intDetail - 1).Format, intNutrientQty, dblNutrientFactor, cLang.GetString(clsEGSLanguage.CodeType.NA))
                                            If dblY = 0 Then
                                                strNutrientVal = "0"
                                            Else
                                                strNutrientVal = dblY.ToString(G_Nutrient(intDetail - 1).Format, userCulture)
                                            End If
                                        Else
                                            strNutrientVal = "-"
                                        End If
                                        xrCNutList = New XRTableCell
                                        xrCNutList.CanShrink = False
                                        xrCNutList.Width = CInt(intHWidth)
                                        xrCNutList.Text = strNutrientVal
                                        xrCNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
                                        xrCNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
                                        xrRNutList.Cells.Add(xrCNutList)
                                    Case 2
                                        If Not IsDBNull(drvNutList(strFieldName)) And Not drvNutList(strFieldName).ToString = "1.#INF" And Not CDbl(drvNutList(strFieldName)) = -1 Then
                                            Dim dblY As Double = fctStrNutrientValue(Format(CType(drvNutList(strFieldName), Double), G_Nutrient(intDetail - 1).Format), G_Nutrient(intDetail - 1).Format, intNutrientQty, dblNutrientFactor, cLang.GetString(clsEGSLanguage.CodeType.NA))
                                            Dim dbly2 As Double = dblY / 4.184
                                            If dbly2 = 0 Then
                                                strNutrientVal = "0"
                                            Else
                                                strNutrientVal = dbly2.ToString(G_Nutrient(intDetail - 1).Format, userCulture)
                                            End If
                                        Else
                                            strNutrientVal = "-"
                                        End If
                                        xrCNutList = New XRTableCell
                                        xrCNutList.CanShrink = False
                                        xrCNutList.Width = CInt(intHWidth)
                                        ' RDC 01.06.2014 : Replaced CDbl(strNutrientVal) to CDblDB(strNutrientVal)
                                        xrCNutList.Text = strNutrientVal
                                        xrCNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
                                        xrCNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
                                        xrRNutList.Cells.Add(xrCNutList)
                                End Select
                            Next
                        Case Else   ' Other Nutrients
                            Dim strNutrientVal As String = ""
                            If Not IsDBNull(drvNutList(strFieldName)) And Not drvNutList(strFieldName).ToString = "1.#INF" And Not CDbl(drvNutList(strFieldName)) = -1 Then
                                Dim dblY As Double = fctStrNutrientValue(Format(CType(drvNutList(strFieldName), Double), G_Nutrient(intDetail - 1).Format), G_Nutrient(intDetail - 1).Format, intNutrientQty, dblNutrientFactor, cLang.GetString(clsEGSLanguage.CodeType.NA))
                                If dblY = 0 Then
                                    strNutrientVal = "0"
                                Else
                                    strNutrientVal = dblY.ToString(G_Nutrient(intDetail - 1).Format, userCulture)
                                End If
                            Else
                                strNutrientVal = "-"
                            End If
                            xrCNutList = New XRTableCell
                            xrCNutList.CanShrink = False
                            xrCNutList.Width = CInt(intHWidth)
                            xrCNutList.Text = strNutrientVal
                            xrCNutList.Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
                            xrCNutList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
                            xrRNutList.Cells.Add(xrCNutList)
                    End Select
                Next
                xrTNutList.Rows.Add(xrRNutList)
            Next

            .Detail.Controls.Add(xrTNutList)
            intCurrentY += (xrTNutList.Height + 10)





            ''MEASURE WIDTH
            'Dim L_lngNutCol(35) As Long '  L_lngNutCol(15) 
            'Dim L_lngNameCol As Long = 0
            'Dim L_lngYieldCol As Long = 0
            'Dim L_lngNutRow As Long = 0

            'Dim KiloCode As Long = fctGetUnitCodeFromType(100)
            'Dim LiterCode As Long = fctGetUnitCodeFromType(200)

            ''Nutrient Header
            'For n As Integer = 1 To L_NutrientCount
            '    strX = G_Nutrient(n).Name
            '    L_lngNutCol(n) = ReportingTextUtils.MeasureText(strX, fntDetail1, 0, sf, Me.Padding).Width
            'Next

            ''Nutrient Values
            'Dim strNA As String = cLang.GetString(clsEGSLanguage.CodeType.NA)
            'For Each drv As DataRowView In dtNutrientList.DefaultView
            '    'Get Factor
            '    Select Case intListeType
            '        Case ID_Merchandise : dblNutrientFactor = 1
            '        Case ID_Recipe 'dblNutrientFactor = fctGetRecipeNutrientFactor(False, CType(drv("YieldUnit"), Long), CType(drv("SrUnit"), Long), CType(drv("SrQty"), Double), CType(drv("srLevel"), Long), KiloCode, LiterCode)
            '            dblNutrientFactor = GetNutrientFactor(CInt(drv("Code")))
            '        Case ID_Menu
            '            If Not IsDBNull(drv("SrWeight")) Then dblNutrientFactor = fctGetMenuNutrientFactor(False, CType(drv("SrWeight"), Double))
            '    End Select

            '    'Get Nutrient Values
            '    For n As Integer = 1 To L_NutrientCount
            '        strX = "N" & n
            '        If Not IsDBNull(drv(strX)) Then
            '            If drv(strX).ToString <> "1.#INF" Then
            '                strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
            '            Else
            '                strX = ""
            '            End If
            '            intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
            '            If intTextWidth > L_lngNutCol(n) Then L_lngNutCol(n) = intTextWidth
            '        End If

            '        If n = 1 Then
            '            strX = "N" & n
            '            If Not IsDBNull(drv(strX)) Then
            '                If drv(strX).ToString <> "1.#INF" Then
            '                    strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
            '                End If

            '                intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
            '                If intTextWidth > L_lngNutCol(n) Then L_lngNutCol(n) = intTextWidth
            '            End If
            '        End If
            '    Next

            '    strX = ""
            '    If Not IsDBNull(drv("YieldName")) Then
            '        strX = Trim(drv("YieldName"))
            '        intTextWidth = ReportingTextUtils.MeasureText(strX, fntDetail2, 0, sf, Me.Padding).Width
            '        If intTextWidth > L_lngYieldCol Then L_lngYieldCol = intTextWidth
            '    End If
            'Next

            ''Recipe Names
            'L_lngNameCol = 0
            'For n As Integer = 1 To L_NutrientCount
            '    L_lngNutCol(n) += intColumnSpace
            '    L_lngNameCol += L_lngNutCol(n)
            '    If n = 1 Then L_lngNameCol += L_lngNutCol(n)
            'Next
            'L_lngYieldCol += intColumnSpace
            'L_lngNameCol += L_lngYieldCol
            'L_lngNameCol = (intAvailableWidth - L_lngNameCol)

            'Dim intColumnHeaderLineWidth As Integer = 0

            ''AGL 2013.05.14
            'Dim lngWidestName As Long = 0
            'For Each drv As DataRowView In dtNutrientList.DefaultView
            '    lngWidestName = fctGetWidest(drv("Name"), intTextHeight, lngWidestName, fntDetail2)
            'Next

            ''AGL 2013.03.14 - to fix printouts when too many nutrients are added
            'If L_lngNameCol < 0 Then
            '    intColumnHeaderLineWidth = L_lngNameCol * -1
            '    If intColumnHeaderLineWidth > intAvailableWidth Then
            '        intColumnHeaderLineWidth += intAvailableWidth + lngWidestName 'AGL 2013.05.14 - added widest name width
            '    End If
            '    L_lngNameCol = 0
            'End If


            ''MEASURE HEIGHT - Nutrient Header
            'For i As Integer = 1 To L_NutrientCount
            '    strX = G_Nutrient(i).Name & " " & G_Nutrient(i).Unit
            '    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngNutCol(i) - intColumnSpace, sf, Me.Padding).Height
            '    If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight

            '    If i = 1 Then 'kcal
            '        strX = G_Nutrient(i).Name & " kcal"
            '        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngNutCol(i) - intColumnSpace, sf, Me.Padding).Height
            '        If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
            '    End If
            'Next

            ''AGL 2013.05.15 - force 20
            'L_lngNutRow = 20


            ''PRINT DETAIL - Nutrient Header
            'intCurrentX = 0
            'intCurrentY = 0

            ''AGL 2013.03.14 - moved here
            ''Line
            '.Detail.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, IIf(intColumnHeaderLineWidth > 0, intColumnHeaderLineWidth, intAvailableWidth), 1)})
            'intCurrentY += 10

            ''AGL 2013.03.15 - change name according to listeType
            'Select Case intListeType
            '    Case ID_Merchandise
            '        strX = cLang.GetString(clsEGSLanguage.CodeType.Merchandise)  'Merchandise
            '    Case ID_Recipe
            '        strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)  'Recipe
            '    Case Else
            '        strX = ""
            'End Select


            ''AGL 2013.05.14
            ''.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNameCol - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, lngWidestName, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            ''intCurrentX += lngWidestName 'L_lngNameCol + L_lngYieldCol

            ''AGL 2013.05.14 
            'intCurrentX += lngWidestName + 5
            'For n As Integer = 1 To L_NutrientCount
            '    strX = G_Nutrient(n).Name & vbCrLf & " " & G_Nutrient(n).Unit
            '    .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '    intCurrentX += L_lngNutCol(n)

            '    If n = 1 Then
            '        'kcal
            '        strX = G_Nutrient(1).Name & vbCrLf & " kcal"
            '        .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '        intCurrentX += L_lngNutCol(n)
            '    End If
            'Next
            'intCurrentX = 0
            'intCurrentY += L_lngNutRow
            ''AGL 2013.03.14 - extended line
            '.Detail.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, IIf(intColumnHeaderLineWidth > 0, intColumnHeaderLineWidth, intAvailableWidth), 3)})
            'intCurrentY += 10

            'L_lngNutRow = 0
            'For Each drv As DataRowView In dtNutrientList.DefaultView
            '    'Get Factor
            '    Select Case intListeType
            '        Case ID_Merchandise : dblNutrientFactor = 1
            '        Case ID_Recipe 'dblNutrientFactor = fctGetRecipeNutrientFactor(False, CType(drv("YieldUnit"), Long), CType(drv("SrUnit"), Long), CType(drv("SrQty"), Double), CType(drv("srLevel"), Long), KiloCode, LiterCode)
            '            dblNutrientFactor = GetNutrientFactor(CInt(drv("Code")))
            '        Case ID_Menu
            '            If Not IsDBNull(drv("SrWeight")) Then dblNutrientFactor = fctGetMenuNutrientFactor(False, CType(drv("SrWeight"), Double))
            '    End Select

            '    'MEASURE HEIGHT - Nutrient Values
            '    For n As Integer = 1 To L_NutrientCount
            '        strX = "N" & n
            '        If Not IsDBNull(drv(strX)) Then
            '            If drv(strX).ToString <> "1.#INF" Then
            '                strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
            '            Else
            '                strX = ""
            '            End If
            '            intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNutCol(n) - intColumnSpace, sf, Me.Padding).Height
            '            If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
            '        End If

            '        If n = 1 Then
            '            strX = "N" & n
            '            If Not IsDBNull(drv(strX)) Then
            '                If drv(strX).ToString <> "1.#INF" Then
            '                    strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
            '                End If
            '                intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNutCol(n) - intColumnSpace, sf, Me.Padding).Height
            '                If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
            '            End If
            '        End If
            '    Next

            '    strX = ""
            '    If Not IsDBNull(drv("YieldName")) Then
            '        strX = Trim(drv("YieldName"))
            '        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngYieldCol - intColumnSpace, sf, Me.Padding).Height
            '        If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
            '    End If

            '    If Not IsDBNull(drv("Name")) Then
            '        strX = VB.Trim(CType(drv("Name"), String))
            '        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNameCol - intColumnSpace, sf, Me.Padding).Height
            '        If intTextHeight > L_lngNutRow Then L_lngNutRow = intTextHeight
            '    End If


            '    'PRINT DETAIL - Nutrient Values
            '    'Recipe Name
            '    If Not IsDBNull(drv("Name")) Then
            '        strX = VB.Trim(CType(drv("Name"), String))
            '        'AGL 2013.05.14
            '        '.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNameCol - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})
            '        .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, lngWidestName, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})

            '    End If
            '    'AGL 2013.05.14
            '    'intCurrentX += L_lngNameCol
            '    intCurrentX += lngWidestName + 5

            '    'Yield 
            '    If Not IsDBNull(drv("YieldName")) Then
            '        strX = VB.Trim(CType(drv("YieldName"), String))
            '        'AGL 2013.05.14
            '        '.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngYieldCol - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})
            '        .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngYieldCol - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, True, True)})
            '    End If
            '    intCurrentX += L_lngYieldCol

            '    'Nutrient
            '    For n As Integer = 1 To L_NutrientCount
            '        strX = "N" & n
            '        If Not IsDBNull(drv(strX)) Then
            '            If drv(strX).ToString <> "1.#INF" Then
            '                strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
            '            Else
            '                strX = ""
            '            End If
            '            'AGL 2013.05.14
            '            '.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '            .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '        End If
            '        intCurrentX += L_lngNutCol(n)

            '        If n = 1 Then
            '            strX = "N" & n
            '            If Not IsDBNull(drv(strX)) Then
            '                If drv(strX).ToString <> "1.#INF" Then
            '                    strX = fctStrNutrientValue(Format(CType(drv(strX), Double), G_Nutrient(n).Format), G_Nutrient(n).Format, intNutrientQty, dblNutrientFactor, strNA)
            '                End If
            '                'AGL 2013.05.14
            '                '.Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, L_lngNutRow, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '                .Detail.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, L_lngNutCol(n) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            '            End If
            '            intCurrentX += L_lngNutCol(n)
            '        End If
            '    Next

            '    'AGL 2013.05.14
            '    intCurrentY += intTextHeight 'L_lngNutRow
            '    intCurrentX = 0
            'Next
        End With
        '--- PAGE DETAIL ---
        Return Me
    End Function

    'fctPrintRecipeMenuList
    'Print the recipe/menu list
    'MCM
    'Last Modified: 15 November 2005
    'Function fctPrintRecipeMenuList(ByVal dtRecipeList As DataTable, ByVal strReportTitle As String, ByVal strRecipeSubHeading As String, ByVal intPageLanguage As Integer, _
    '                   ByVal blIncludeNumber As Boolean, _
    '                   ByVal blIncludeCostOfGoods As Boolean, ByVal blIncludeFactor As Boolean, ByVal blIncludeTax As Boolean, _
    '                   ByVal blIncludeSellingPrice As Boolean, ByVal blIncludeImposedPrice As Boolean, ByVal blIncludeDate As Boolean, _
    '                   ByVal strFontName As String, ByVal sgFontSize As Single, _
    '                   ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double, _
    '                   ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, Optional ByVal blLandscape As Boolean = False, _
    '                   Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16) As XtraReport 'VRP 05.11.2007

    '    'On Error GoTo err_fctPrintRecipeListLow
    '    Dim drvReport As DataRowView
    '    Dim strCoeffFormat As String
    '    Dim strLastCurrency As String = Nothing
    '    Dim strX As String
    '    Dim blnOneCurrency As Boolean
    '    Dim cLang As New clsEGSLanguage(intPageLanguage)
    '    G_Options.FactorType = 0

    '    Cursor.Current = Cursors.WaitCursor

    '    If G_Options.FactorType > 0 Then
    '        strCoeffFormat = G_FormatOneDecimal
    '    Else
    '        strCoeffFormat = G_FormatTwoDecimal
    '    End If

    '    sf = New StringFormat(StringFormatFlags.NoClip)
    '    Try
    '        'If G_ReportOptions.blnPictureOneRight Then
    '        '    fntReportTitle = New Font(strFontName, 14, FontStyle.Bold)
    '        'Else
    '        '    fntReportTitle = New Font(strFontName, 16, FontStyle.Bold)
    '        'End If

    '        fntReportTitle = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'VRP 05.11.2007
    '        fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
    '        fntBold = New Font(strFontName, sgFontSize, FontStyle.Bold)
    '        fntFooter = New Font(strFontName, 8, FontStyle.Bold)
    '    Catch ex As Exception
    '        Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
    '        Return Me
    '    End Try
    '    xrLblEGS.Font = fntFooter

    '    If dtRecipeList.DefaultView.Count > 0 Then
    '        With Me
    '            Me.DataMember = dtRecipeList.TableName.ToString
    '            Me.DataSource = dtRecipeList

    '            'Papersize
    '            '------------
    '            .PaperKind = Printing.PaperKind.Custom
    '            .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

    '            'Margins
    '            '------------
    '            .Margins.Left = CInt(dblLeftMargin)
    '            .Margins.Top = CInt(dblTopMargin)
    '            .Margins.Bottom = CInt(dblBottomMargin)
    '            .Margins.Right = CInt(dblRightMargin)
    '            '                .PageFooter.Height = dblBottomMargin


    '            'Orientation
    '            '-----------
    '            If blLandscape Then
    '                .Landscape = True
    '                intAvailableWidth = .PageHeight
    '            Else
    '                .Landscape = False
    '                intAvailableWidth = .PageWidth
    '            End If

    '            intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin) - 15


    '            .xrLinePF.Left = 0
    '            .xrLinePF.Width = intAvailableWidth
    '            .xrPIPageNumber.Left = intAvailableWidth / 2
    '            .xrPIPageNumber.Width = intAvailableWidth / 2
    '            '.xrPIPageNumber.Font = fntBold
    '            .xrLblEGS.Left = 0
    '            .xrLblEGS.Width = intAvailableWidth / 2
    '            '.xrLblEGS.Font = fntBold
    '            .xrLinePF.Visible = True
    '            .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"


    '            If NoPrintLines Then
    '                .xrLinePF.Visible = False
    '            End If
    '            subReportFooter(intAvailableWidth, strFontName)


    '            intCurrentX = 0
    '            intCurrentY = 0

    '            'Report Title
    '            intTextHeight = ReportingTextUtils.MeasureText(strReportTitle, fntReportTitle, intAvailableWidth, sf, Me.Padding).Height
    '            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strReportTitle, fntReportTitle, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})

    '            intCurrentY = intTextHeight
    '            'Sub Report Header
    '            intTextHeight = ReportingTextUtils.MeasureText(strRecipeSubHeading, fntRegular, intAvailableWidth, sf, Me.Padding).Height
    '            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strRecipeSubHeading, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

    '            'Column Header
    '            sf = New StringFormat(StringFormatFlags.DirectionVertical)

    '            'Number
    '            strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
    '            L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2

    '            'Cost of Goods
    '            strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
    '            L_lngCol(2) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2

    '            Select Case G_Options.FactorType
    '                Case 0
    '                    strX = cLang.GetString(clsEGSLanguage.CodeType.Const)
    '                    L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2
    '                Case 1
    '                    strX = cLang.GetString(clsEGSLanguage.CodeType.Profit) '"Profit"
    '                    L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2
    '                Case 2
    '                    strX = cLang.GetString(clsEGSLanguage.CodeType.FC)    '"FC"     
    '                    L_lngCol(3) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2
    '            End Select

    '            strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
    '            L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2

    '            strX = cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
    '            L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2

    '            strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
    '            L_lngCol(6) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2

    '            strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
    '            L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2

    '            L_lngCol(8) = ReportingTextUtils.MeasureText("WWW", fntBold, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 2

    '            blnOneCurrency = True

    '            For Each drvReport In dtRecipeList.DefaultView
    '                strLastCurrency = drvReport("Currency").ToString
    '                strX = Trim(CType(drvReport("Name"), String))

    '                strX = drvReport("Number").ToString
    '                strX = Replace(strX, Chr(1), "")
    '                If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(1) Then L_lngCol(1) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)

    '                If Not IsDBNull(drvReport("calcprice")) Then
    '                    strX = Format(CType(drvReport("calcprice"), Double) + 0, G_strPriceFormat)
    '                    If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(2) Then L_lngCol(2) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)
    '                End If

    '                If Not IsDBNull(drvReport("coeff")) Then
    '                    strX = Format(fctConvertCoeff(CType(drvReport("coeff"), Double), G_Options.FactorType), strCoeffFormat)
    '                    If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(3) Then L_lngCol(3) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)
    '                End If

    '                If Not IsDBNull(drvReport("Tax")) Then
    '                    strX = Format(drvReport("Tax"), G_FormatOneDecimal) + 0 & "%"
    '                    'strX = drvReport("Tax").ToString & "%"
    '                    If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(4) Then L_lngCol(4) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)
    '                End If

    '                If Not IsDBNull(drvReport("calcprice")) And Not IsDBNull(drvReport("coeff")) Then
    '                    strX = Format(CType(drvReport("calcprice"), Double) * CType(drvReport("coeff"), Double) + 0, G_strPriceFormat)
    '                    If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(5) Then L_lngCol(5) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)
    '                End If

    '                If Not IsDBNull(drvReport("ImposedPrice")) Then
    '                    strX = Format(CType(drvReport("ImposedPrice"), Double) + 0, G_strPriceFormat)
    '                    If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(6) Then L_lngCol(6) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)
    '                End If

    '                strX = ""
    '                If Not IsDBNull(drvReport("Dates")) Then strX = fctConvertDate(CType(drvReport("Dates"), Date))
    '                If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(7) Then L_lngCol(7) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)

    '                'strX = drReport("Currency").ToString
    '                If drvReport("Currency").ToString <> "" Then
    '                    If strLastCurrency <> drvReport("Currency").ToString Then
    '                        blnOneCurrency = False
    '                        strLastCurrency = drvReport("Currency").ToString
    '                    End If
    '                End If
    '                strX = strLastCurrency
    '                If (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace) > L_lngCol(8) Then L_lngCol(8) = (.MeasureText(strX, fntRegular, intTextHeight, sf, Me.Padding).Height + intColumnSpace)
    '            Next

    '            If blnOneCurrency Then
    '                L_lngCol(8) = 0
    '            Else
    '                'strLastCurrency = ""
    '            End If

    '            If Not blIncludeNumber Then L_lngCol(1) = 0
    '            If Not blIncludeCostOfGoods Then L_lngCol(2) = 0
    '            If Not blIncludeFactor Then L_lngCol(3) = 0
    '            If Not blIncludeTax Then L_lngCol(4) = 0
    '            If Not blIncludeSellingPrice Then L_lngCol(5) = 0
    '            If Not blIncludeImposedPrice Then L_lngCol(6) = 0
    '            If Not blIncludeDate Then L_lngCol(7) = 0
    '            If Not blIncludeCostOfGoods And Not blIncludeSellingPrice And Not blIncludeImposedPrice Then L_lngCol(8) = 0

    '            L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1))
    '            L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(2))
    '            L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(3))
    '            L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(4))
    '            L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(5))
    '            L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(6))
    '            L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(7))
    '            '  L_lngNameW = Math.Abs(L_lngNameW - L_lngCol(8))

    '            intCurrentY += intTextHeight + 20
    '            intCurrentX = 0

    '            'Column Header
    '            Dim strCoeff As String = Nothing
    '            Dim strCurrencyHeading As String = Nothing

    '            If strLastCurrency <> "" Then strCurrencyHeading = vbCrLf & strLastCurrency

    '            Select Case G_Options.FactorType
    '                Case 0 : strCoeff = cLang.GetString(clsEGSLanguage.CodeType.Const)      'Const
    '                Case 1 : strCoeff = cLang.GetString(clsEGSLanguage.CodeType.Profit)     'Profit
    '                Case 2 : strCoeff = cLang.GetString(clsEGSLanguage.CodeType.FC)         'FC 
    '            End Select

    '            sf = New StringFormat(StringFormatFlags.NoClip)

    '            If blIncludeNumber Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
    '                intTextHeight = ReportingTextUtils.MeasureText(strX, fntRegular, L_lngCol(1) - intColumnSpace, sf, Me.Padding).Height
    '            End If

    '            strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
    '            intTextHeight = fctGetHighest(strX, intTextHeight, L_lngNameW - intColumnSpace, fntRegular, sf)

    '            If blIncludeCostOfGoods Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods) & strCurrencyHeading
    '                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(2) - intColumnSpace, fntRegular, sf)
    '            End If

    '            If blIncludeFactor Then
    '                intTextHeight = fctGetHighest(strCoeff, intTextHeight, L_lngCol(3) - intColumnSpace, fntRegular, sf)
    '            End If

    '            If blIncludeTax Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
    '                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(4) - intColumnSpace, fntRegular, sf)
    '            End If

    '            If blIncludeSellingPrice Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.SellingPricePlusTax) & vbCrLf & strCurrencyHeading
    '                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(5) - intColumnSpace, fntRegular, sf) + intColumnSpace
    '            End If

    '            If blIncludeImposedPrice Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
    '                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(6) - intColumnSpace, fntRegular, sf)
    '            End If

    '            If blIncludeDate Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
    '                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(7) - intColumnSpace, fntRegular, sf)
    '            End If

    '            .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
    '            intCurrentY += 10

    '            If blIncludeNumber Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
    '                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
    '            End If

    '            intCurrentX += L_lngCol(1)
    '            strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
    '            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

    '            intCurrentX += L_lngNameW
    '            If blIncludeCostOfGoods Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
    '                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & strCurrencyHeading, fntBold, intCurrentX, intCurrentY, L_lngCol(2), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
    '            End If

    '            intCurrentX += L_lngCol(2)
    '            If blIncludeFactor Then
    '                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strCoeff, fntBold, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '            End If

    '            intCurrentX += L_lngCol(3)
    '            If blIncludeTax Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
    '                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '            End If

    '            intCurrentX += L_lngCol(4)

    '            If blIncludeSellingPrice Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.SellingPricePlusTax)
    '                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & strCurrencyHeading, fntBold, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
    '            End If

    '            intCurrentX += L_lngCol(5)

    '            If blIncludeImposedPrice Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
    '                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & strCurrencyHeading, fntBold, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
    '            End If

    '            intCurrentX += L_lngCol(6)

    '            If blIncludeDate Then
    '                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
    '                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBold, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '            End If
    '            'End Of Column Header


    '            intCurrentX = 0
    '            intCurrentY += intTextHeight
    '            .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})
    '            'intCurrentY += 10


    '            Dim intMaxHeight As Integer
    '            Dim sgFontSizeTemp As Single
    '            Dim intTextHeightTemp As Integer
    '            Dim strCategory As String = Nothing
    '            Dim strGroupHeader As String = Nothing
    '            Dim blnNewGroup As Boolean
    '            intCurrentY = 17

    '            intMaxHeight = ReportingTextUtils.MeasureText("A", fntRegular, intAvailableWidth, sf, Me.Padding).Height
    '            sf = New StringFormat(StringFormatFlags.MeasureTrailingSpaces)
    '            For Each drvReport In dtRecipeList.DefaultView
    '                blnNewGroup = False
    '                intCurrentX = 0
    '                fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
    '                sgFontSizeTemp = sgFontSize

    '                If G_ReportOptions.strSortBy = "CategoryName" Then
    '                    strX = CType(drvReport("CategoryName"), String)
    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                ElseIf G_ReportOptions.strSortBy = "Dates" And Not blIncludeDate Then
    '                    strX = fctConvertDate(CType(drvReport("Dates"), Date))
    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                ElseIf G_ReportOptions.strSortBy = "Number" And Not blIncludeNumber Then
    '                    strX = drvReport("Number").ToString
    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                ElseIf G_ReportOptions.strSortBy = "Tax" And Not blIncludeTax Then
    '                    strX = Format(drvReport("Tax"), G_FormatOneDecimal) & "%"
    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                ElseIf G_ReportOptions.strSortBy = "CalcPrice" And Not blIncludeCostOfGoods Then
    '                    If Not IsDBNull(drvReport("calcprice")) Then
    '                        strX = Format(CType(drvReport("calcprice"), Double) + 0, G_strPriceFormat)
    '                    Else
    '                        strX = "---"
    '                    End If

    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                ElseIf G_ReportOptions.strSortBy = "Const" And Not blIncludeFactor Then
    '                    If Not IsDBNull(drvReport("coeff")) Then
    '                        strX = Format(fctConvertCoeff(CType(drvReport("coeff"), Double), G_Options.FactorType), strCoeffFormat)
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                ElseIf G_ReportOptions.strSortBy = "SellingPrice" And Not blIncludeSellingPrice Then
    '                    If Not IsDBNull(drvReport("calcprice")) And Not IsDBNull(drvReport("coeff")) Then
    '                        strX = Format(Format(CType(drvReport("calcprice"), Double) + 0, G_strPriceFormat) * Format(fctConvertCoeff(CType(drvReport("coeff"), Double), G_Options.FactorType), strCoeffFormat) * (1 + (Format(drvReport("Tax"), G_FormatOneDecimal) / 100)) + 0, G_strPriceFormat)
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                ElseIf G_ReportOptions.strSortBy = "ImposedPrice" And Not blIncludeImposedPrice Then
    '                    If Not IsDBNull(drvReport("ImposedPrice")) Then
    '                        strX = Format(CType(drvReport("ImposedPrice"), Double) + 0, G_strPriceFormat)
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    If UCase(strGroupHeader) <> UCase(strX) Then
    '                        strGroupHeader = strX
    '                        blnNewGroup = True
    '                    End If
    '                End If

    '                If blnNewGroup Then
    '                    intCurrentY += 5
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroupHeader, fntBold, intCurrentX, intCurrentY, intAvailableWidth, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
    '                    intCurrentY += intTextHeight + 5
    '                End If

    '                If blIncludeNumber Then
    '                    strX = drvReport("Number").ToString
    '                    strX = Replace(strX, Chr(1), "")
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
    '                End If
    '                intCurrentX += L_lngCol(1)

    '                strX = Trim(CType(drvReport("Name"), String))

    '                intTextHeightTemp = CInt(.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
    '                intTextHeight = intTextHeightTemp
    '                If G_ReportOptions.blShrinkToFit Then
    '                    Do While Not intMaxHeight >= CInt(.MeasureText(strX, fntRegular, L_lngNameW - intColumnSpace, sf, Me.Padding).Height)
    '                        sgFontSizeTemp -= 1
    '                        fntRegular = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
    '                    Loop
    '                    intTextHeight = intMaxHeight
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
    '                Else
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngNameW - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
    '                End If


    '                fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)

    '                intCurrentX += L_lngNameW
    '                If blIncludeCostOfGoods Then
    '                    If Not IsDBNull(drvReport("calcprice")) Then
    '                        strX = Format(CType(drvReport("calcprice"), Double) + 0, G_strPriceFormat)
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '                End If

    '                intCurrentX += L_lngCol(2)
    '                If blIncludeFactor Then
    '                    If Not IsDBNull(drvReport("coeff")) Then
    '                        strX = Format(fctConvertCoeff(CType(drvReport("coeff"), Double), G_Options.FactorType), strCoeffFormat)
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '                End If

    '                intCurrentX += L_lngCol(3)
    '                If blIncludeTax Then
    '                    If Not IsDBNull(drvReport("Tax")) Then
    '                        strX = Format(drvReport("Tax"), G_FormatOneDecimal) + 0 & "%"
    '                        'strX = drvReport("Tax").ToString + 0 & "%"
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '                End If

    '                intCurrentX += L_lngCol(4)
    '                If blIncludeSellingPrice Then
    '                    If Not IsDBNull(drvReport("calcprice")) And Not IsDBNull(drvReport("coeff")) Then
    '                        strX = Format(Format(CType(drvReport("calcprice"), Double) + 0, G_strPriceFormat) * Format(fctConvertCoeff(CType(drvReport("coeff"), Double), G_Options.FactorType), strCoeffFormat) * (1 + (Format(drvReport("Tax"), G_FormatOneDecimal) / 100)) + 0, G_strPriceFormat)
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '                End If

    '                intCurrentX += L_lngCol(5)
    '                If blIncludeImposedPrice Then
    '                    If Not IsDBNull(drvReport("ImposedPrice")) Then
    '                        strX = Format(CType(drvReport("ImposedPrice"), Double) + 0, G_strPriceFormat)
    '                    Else
    '                        strX = "---"
    '                    End If
    '                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '                End If

    '                intCurrentX += L_lngCol(6)
    '                If blIncludeDate Then
    '                    If Not IsDBNull(drvReport("Dates")) Then
    '                        strX = fctConvertDate(CType(drvReport("Dates"), Date))
    '                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntRegular, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intMaxHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
    '                    End If
    '                End If

    '                'intCurrentY += intTextHeight 
    '                intCurrentY += GetLineSpace(intTextHeight)
    '            Next
    '            dtRecipeList.Reset() 'VRP 11.03.2008
    '            Return Me
    '        End With
    '    End If


    '    Exit Function
    '    'err_fctPrintRecipeListLow:
    '    '       MsgBox(Err.Description)
    'End Function

    'VRP 27.07.2009
    Function PrintRecipeMenuListCoop(ByVal dtRecipeList As DataTable, ByVal strReportTitle As String,
                                ByVal strRecipeSubHeading As String, ByVal intPageLanguage As Integer,
                                ByVal blnIncludeNumber As Boolean, ByVal blnIncludeCostOfGoods As Boolean,
                                ByVal blnIncludeFactor As Boolean, ByVal blnIncludeTax As Boolean,
                                ByVal blnIncludeSellingPrice As Boolean, ByVal blnIncludeImposedPrice As Boolean,
                                ByVal blnIncludeDate As Boolean, ByVal strFontName As String, ByVal sgFontSize As Single,
                                ByVal dblPageWidth As Double, ByVal dblPageHeight As Double,
                                ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double,
                                ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double,
                                Optional ByVal blLandscape As Boolean = False,
                                Optional ByVal strFontTitleName As String = "Arial",
                                Optional ByVal sgFontTitleSize As Single = 16,
                                Optional blnIncludeName As Boolean = False,
                                Optional blnIncludeSubName As Boolean = False,
                                Optional blnIncludeCategory As Boolean = False, Optional ByVal userLocale As String = "en-US") As XtraReport

        If Not dtRecipeList.Rows.Count > 0 Then Return Nothing
        Dim strX As String
        Dim cLang As New clsEGSLanguage(intPageLanguage)
        Cursor.Current = Cursors.WaitCursor
        sf = New StringFormat(StringFormatFlags.NoClip)

        '--- SETTINGS ---
        Dim fntTitle1 As Font = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'Title
        Dim fntTitle2 As Font = New Font(strFontName, sgFontSize, FontStyle.Regular) 'Sub Title
        Dim fntTitle3 As Font = New Font(strFontName, sgFontSize, FontStyle.Bold) 'Column Title
        Dim fntDetail1 As Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
        Dim fntDetail2 As Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
        Dim fntFooter1 As Font = New Font(strFontName, 8, FontStyle.Bold)

        Dim lblsubtitle As String
        Dim lblfooter As String

        Dim userCulture As CultureInfo = New CultureInfo(userLocale)

        Try
            lblsubtitle = dtRecipeList.Rows(0)("Subtitle").ToString()
            lblfooter = dtRecipeList.Rows(0)("Footer").ToString()
        Catch ex As Exception

        End Try


        With Me 'paper , orientation, width
            '.DataMember = dtRecipeList.TableName.ToString
            '.DataSource = dtRecipeList

            .PaperKind = Printing.PaperKind.Custom
            .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

            .Margins.Left = CInt(dblLeftMargin)
            .Margins.Top = CInt(dblTopMargin)
            .Margins.Bottom = CInt(dblBottomMargin)
            .Margins.Right = CInt(dblRightMargin)

            If blLandscape Then
                .Landscape = True
                intAvailableWidth = .PageHeight
            Else
                .Landscape = False
                intAvailableWidth = .PageWidth
            End If

            intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin) - 15
        End With
        '--- SETTINGS ---

        '--- PAGE HEADER ----
        With Me
            intCurrentX = 0
            intCurrentY = 0

            '>>Report Title<<
            strX = strReportTitle
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle1, intAvailableWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle1, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})
            intCurrentY = intTextHeight + 5

            '>>SubReport Header<<
            strX = lblsubtitle '"Sample recipes from April 1, 2016 - April 7, 2016" 'LD20160507 Sample
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle2, intAvailableWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle2, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentY += intTextHeight + 5

            '>>SubReport Header<<
            strX = strRecipeSubHeading
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle2, intAvailableWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle2, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom, , , 1)})
            intCurrentY += intTextHeight + 10
            .lblcopyright.Text = lblfooter '"Content is copyright c Vita-Mixx Corporation. All rights reserved."


            ''>>Line<<
            '.PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
            'intCurrentY += 10


            .lblcopyright.Visible = True
            .xrLineReport.Visible = True

            '>>Column Header<<
            sf = New StringFormat(StringFormatFlags.DirectionVertical)
            strX = cLang.GetString(clsEGSLanguage.CodeType.Category)
            L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
            L_lngCol(2) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
            L_lngCol(3) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
            L_lngCol(4) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace + 25

            strX = cLang.GetString(clsEGSLanguage.CodeType.Const)
            L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
            L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
            L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
            L_lngCol(8) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = "WWW"
            L_lngCol(9) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace




            Dim strLastCurrency As String = Nothing
            Dim blnOneCurrency As Boolean = True
            'For Each row As DataRowView In dtRecipeList.DefaultView

            'Next

            If blnOneCurrency Then L_lngCol(9) = 0
            If Not blnIncludeCategory Then L_lngCol(1) = 0
            If Not blnIncludeNumber Then L_lngCol(2) = 0
            If Not blnIncludeCostOfGoods Then L_lngCol(3) = 0
            If Not blnIncludeDate Then L_lngCol(4) = 0
            If Not blnIncludeFactor Then L_lngCol(5) = 0
            If Not blnIncludeTax Then L_lngCol(6) = 0
            If Not blnIncludeSellingPrice Then L_lngCol(7) = 0
            If Not blnIncludeImposedPrice Then L_lngCol(8) = 0

            Dim widthForName As Integer = 100      'EDIT HERE (MINIMUM WIDTH FOR COLUMN 'NAME')
            Dim minWidth As Integer = 60           'EDIT HERE (MINIMUM WIDTH FOR OTHER COLUMNS STATED ABOVE)

            If intAvailableWidth < (L_lngCol(1) + L_lngCol(2) + L_lngCol(3) + L_lngCol(4) + L_lngCol(5) + L_lngCol(6) + L_lngCol(7) + L_lngCol(8) + widthForName) Then
                Dim tmpSub As Integer = (L_lngCol(1) + L_lngCol(2) + L_lngCol(3) + L_lngCol(4) + L_lngCol(5) + L_lngCol(6) + L_lngCol(7) + L_lngCol(8) + widthForName) - (intAvailableWidth)

                Dim countDivisor As Integer = 0
                Dim excessWidth As Integer = 0
                Dim LowerMinimum As New ArrayList
                Dim AboveMinimum As New ArrayList

                'count columns that has width > 0
                For Each col In L_lngCol
                    If col > 0 Then
                        countDivisor += 1
                    End If
                Next

                If countDivisor > 8 Then countDivisor = 8
                If countDivisor < 1 Then countDivisor = 1

                'width to be subtracted
                tmpSub /= countDivisor

                countDivisor = 8

                If blnIncludeCategory Then
                    L_lngCol(1) -= tmpSub
                    If L_lngCol(1) < minWidth Then
                        excessWidth += minWidth - L_lngCol(1)
                        L_lngCol(1) = minWidth
                        LowerMinimum.Add(1)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(1)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeNumber Then 'Cost of Goods
                    L_lngCol(2) -= tmpSub
                    If L_lngCol(2) < minWidth Then
                        excessWidth += minWidth - L_lngCol(2)
                        L_lngCol(2) = minWidth
                        LowerMinimum.Add(2)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(2)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeCostOfGoods Then 'Cost of Goods
                    L_lngCol(3) -= tmpSub
                    If L_lngCol(3) < minWidth Then
                        excessWidth += minWidth - L_lngCol(3)
                        L_lngCol(3) = minWidth
                        LowerMinimum.Add(3)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(3)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeDate Then
                    L_lngCol(4) -= tmpSub
                    If L_lngCol(4) < minWidth Then
                        excessWidth += minWidth - L_lngCol(4)
                        L_lngCol(4) = minWidth
                        LowerMinimum.Add(4)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(4)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeFactor Then 'Factor
                    L_lngCol(5) -= tmpSub
                    If L_lngCol(5) < minWidth Then
                        excessWidth += minWidth - L_lngCol(5)
                        L_lngCol(5) = minWidth
                        LowerMinimum.Add(5)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(5)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeTax Then 'tax
                    L_lngCol(6) -= tmpSub
                    If L_lngCol(6) < minWidth Then
                        excessWidth += minWidth - L_lngCol(6)
                        L_lngCol(6) = minWidth
                        LowerMinimum.Add(6)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(6)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeSellingPrice Then 'Selling Price
                    L_lngCol(7) -= tmpSub
                    If L_lngCol(7) < minWidth Then
                        excessWidth += minWidth - L_lngCol(7)
                        L_lngCol(7) = minWidth
                        LowerMinimum.Add(7)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(7)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeImposedPrice Then 'Imposed Selling Price
                    L_lngCol(8) -= tmpSub
                    If L_lngCol(8) < minWidth Then
                        excessWidth += minWidth - L_lngCol(8)
                        L_lngCol(8) = minWidth
                        LowerMinimum.Add(8)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(8)
                    End If
                Else
                    countDivisor -= 1
                End If

                If LowerMinimum.Count > 0 Then

                    Dim tmpWidth2 As Integer = Math.Ceiling(excessWidth / countDivisor)
                    Dim dblComputedValue() As Double = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                    Dim excessWidth2 As Integer = 0
                    Dim colCount As Integer = 0

                    For Each col In AboveMinimum
                        dblComputedValue(col) = L_lngCol(col)
                    Next

                    'check all columns that are capable to subract width again
                    While colCount <> countDivisor
                        For c = 0 To dblComputedValue.Count - 1
                            If dblComputedValue(c) > 0 Then
                                If dblComputedValue(c) - tmpWidth2 < minWidth Then
                                    dblComputedValue(c) = 0
                                Else
                                    colCount += 1
                                End If
                            End If
                        Next

                        If colCount <> countDivisor Then
                            countDivisor -= 1
                            tmpWidth2 = excessWidth / countDivisor
                            colCount = 0
                        End If

                    End While

                    'subtract width of columns
                    If colCount > 0 Then
                        tmpWidth2 = Math.Ceiling(excessWidth / countDivisor)
                        For c = 0 To dblComputedValue.Count - 1
                            If dblComputedValue(c) > 0 Then
                                L_lngCol(c) -= tmpWidth2
                            End If
                        Next
                    End If
                End If
            End If

            L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1) - L_lngCol(2) - L_lngCol(3) - L_lngCol(4) - L_lngCol(5) - L_lngCol(6) - L_lngCol(7) - L_lngCol(8))


            If blnIncludeName And Not blnIncludeSubName Then
                L_lngName = L_lngNameW
            ElseIf Not blnIncludeName And blnIncludeSubName Then
                L_lngSubName = L_lngNameW
            ElseIf Not blnIncludeName And Not blnIncludeSubName Then
                L_lngName = 0
                L_lngSubName = 0

                If blnIncludeCategory Then
                    L_lngCol(1) = L_lngNameW
                Else
                    Dim inclde As Integer = 0
                    If blnIncludeCategory Then inclde += 1
                    If blnIncludeNumber Then inclde += 1
                    If blnIncludeCostOfGoods Then inclde += 1
                    If blnIncludeDate Then inclde += 1
                    If blnIncludeFactor Then inclde += 1
                    If blnIncludeTax Then inclde += 1
                    If blnIncludeSellingPrice Then inclde += 1
                    If blnIncludeImposedPrice Then inclde += 1

                    If blnIncludeCategory Then L_lngCol(1) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(1) = 0
                    If blnIncludeNumber Then L_lngCol(2) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(2) = 0
                    If blnIncludeCostOfGoods Then L_lngCol(3) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(3) = 0
                    If blnIncludeDate Then L_lngCol(4) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(4) = 0
                    If blnIncludeFactor Then L_lngCol(5) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(5) = 0
                    If blnIncludeTax Then L_lngCol(6) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(6) = 0
                    If blnIncludeSellingPrice Then L_lngCol(7) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(7) = 0
                    If blnIncludeImposedPrice Then L_lngCol(8) += Math.Abs(L_lngNameW / inclde) Else L_lngCol(8) = 0
                End If


            Else
                L_lngName = Math.Abs(L_lngNameW / 2)
                L_lngSubName = Math.Abs(L_lngNameW / 2)
            End If

            'Measure Column Header Height
            sf = New StringFormat(StringFormatFlags.NoClip)
            intTextHeight = 0



            'Category
            If blnIncludeCategory Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Category)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(1) - intColumnSpace, fntTitle3, sf)
            End If

            'SubName
            If blnIncludeSubName Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.SubName)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngSubName - intColumnSpace, fntTitle3, sf)
            End If

            'Recipe Name
            If blnIncludeName Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngName - intColumnSpace, fntTitle3, sf)
            End If

            'Number
            If blnIncludeNumber Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle3, L_lngCol(2) - intColumnSpace, sf, Me.Padding).Height
            End If

            'Cost Of Goods
            Dim strCurrencyHeading As String = vbCrLf & " (" & CStrDB(dtRecipeList.Rows(0)("Currency")) & ")"
            ' If strLastCurrency <> "" Then strCurrencyHeading = vbCrLf & strLastCurrency
            If blnIncludeCostOfGoods Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods) & strCurrencyHeading
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(3) - intColumnSpace, fntTitle3, sf)
            End If

            If blnIncludeDate Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(4) - intColumnSpace, fntTitle3, sf)
            End If

            'Factor
            If blnIncludeFactor Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Const)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(5) - intColumnSpace, fntTitle3, sf)
            End If

            'Tax
            If blnIncludeTax Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(6) - intColumnSpace + 10, fntTitle3, sf)
            End If


            'Selling Price
            If blnIncludeSellingPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.SellingPricePlusTax) & vbCrLf & strCurrencyHeading
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(7) - intColumnSpace, fntTitle3, sf)
            End If

            'Imposed Selling Price
            If blnIncludeImposedPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice) & vbCrLf & strCurrencyHeading
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(8) - intColumnSpace, fntTitle3, sf)
            End If



            'Print Column Header
            If blnIncludeCategory Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Category)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
            End If
            intCurrentX += L_lngCol(1)

            If blnIncludeSubName Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.SubName)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngSubName - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            End If
            intCurrentX += L_lngSubName


            If blnIncludeName Then
                If G_ReportOptions.blnRecipe Then
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
                Else
                    strX = cLang.GetString(clsEGSLanguage.CodeType.Menu)
                End If
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngName - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            End If
            intCurrentX += L_lngName


            If blnIncludeNumber Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            End If
            intCurrentX += L_lngCol(2)

            If blnIncludeCostOfGoods Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & strCurrencyHeading, fntTitle3, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
            End If
            intCurrentX += L_lngCol(3)


            If blnIncludeDate Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace + 10, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If
            intCurrentX += L_lngCol(4) + 10

            If blnIncludeFactor Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Const)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If
            intCurrentX += L_lngCol(5)


            If blnIncludeTax Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If
            intCurrentX += L_lngCol(6)


            If blnIncludeSellingPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & strCurrencyHeading, fntTitle3, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
            End If
            intCurrentX += L_lngCol(7)

            If blnIncludeImposedPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & strCurrencyHeading, fntTitle3, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace + 10, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
            End If


            intCurrentX = 0
            intCurrentY += intTextHeight
            '.PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})


            strX = " "
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle2, intAvailableWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle2, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom, , , 1)})
            intCurrentY += 10

            ''>>Line<<
            '.PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
            'intCurrentY += 10


        End With
        '--- PAGE HEADER ----

        '--- PAGE DETAIL ----
        With Me
            intCurrentX = 0
            intCurrentY = 0
            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel("", fntDetail1, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentY += GetLineSpace(intTextHeight)

            sf = New StringFormat(StringFormatFlags.MeasureTrailingSpaces)

            Dim dvRecipeList As New DataView(dtRecipeList)
            dvRecipeList.Sort = G_ReportOptions.strSortBy
            dtRecipeList = dvRecipeList.ToTable

            Dim intRowHeight As Integer = 0
            Dim sgFontSizeTemp As Single
            Dim strGroupHeader As String = ""
            Dim blnNewGroup As Boolean = False

            For Each row As DataRowView In dtRecipeList.DefaultView
                intCurrentX = 0
                sgFontSizeTemp = sgFontSize


                Dim strPriceFormat As String = CType(row("CurFormat"), String)
                'MEASURE ROW HEIGHT ====

                'Category
                If blnIncludeCategory Then
                    strX = Trim(CType(row("CategoryName"), String))
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngCol(1) - intColumnSpace, sf, Me.Padding).Height
                    If G_ReportOptions.blShrinkToFit Then
                        Do While Not intRowHeight >= intTextHeight
                            sgFontSizeTemp -= 1
                            fntDetail2 = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                        Loop
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    Else
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    End If
                End If

                'SubName
                If blnIncludeSubName Then
                    strX = Trim(CType(row("SubName"), String))
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngSubName - intColumnSpace, sf, Me.Padding).Height
                    If G_ReportOptions.blShrinkToFit Then
                        Do While Not intRowHeight >= intTextHeight
                            sgFontSizeTemp -= 1
                            fntDetail2 = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                        Loop
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    Else
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    End If
                End If

                'Name
                If blnIncludeName Then
                    strX = Trim(CType(row("Name"), String))
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngName - intColumnSpace, sf, Me.Padding).Height
                    If G_ReportOptions.blShrinkToFit Then
                        Do While Not intRowHeight >= intTextHeight
                            sgFontSizeTemp -= 1
                            fntDetail2 = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                        Loop
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    Else
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    End If
                End If


                'Number
                If blnIncludeNumber Then
                    strX = Replace(row("Number").ToString, Chr(1), "")
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(2) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Cost Of Goods
                If blnIncludeCostOfGoods Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("calcprice2")) Then
                            Dim dblAmount As Double = CDblDB(row("calcprice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            'strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                            strX = (CType(dblAmount, Double) + 0).ToString(strPriceFormat, userCulture)

                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("calcprice")) Then
                            strX = Format(CType(row("calcprice"), Double) + 0, strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(3) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Date
                If blnIncludeDate Then
                    If Not IsDBNull(row("Dates")) Then
                        'strX = fctConvertDate(CType(row("Dates"), Date))
                        'strX = DirectCast(row("Dates"), DateTime).ToShortDateString 'JBQL format date automatically getting the culture info
                        strX = DirectCast(row("Dates"), DateTime).ToString("d", userCulture) 'JBQL format date automatically getting the culture info
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(4) - intColumnSpace, sf, Me.Padding).Height
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    End If
                End If


                'Factor
                If blnIncludeFactor Then
                    If Not IsDBNull(row("coeff")) Then
                        strX = Format(fctConvertCoeff(CType(row("coeff"), Double), G_Options.FactorType), G_FormatTwoDecimal)
                    Else
                        strX = "---"
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(5) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Tax
                If blnIncludeTax Then
                    If Not IsDBNull(row("Tax")) Then
                        strX = Format(row("Tax"), G_FormatOneDecimal) + 0 & "%"
                    Else
                        strX = "---"
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(6) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Selling Price
                If blnIncludeSellingPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("sellingPrice2")) Then

                            Dim dblAmount As Double = CDblDB(row("sellingPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            'strX = Format(CType(dblAmount, Double), strPriceFormat) 'KMQDC 2016.09.14
                            strX = dblAmount.ToString(strPriceFormat, userCulture)
                            'strX = Format(CType(row("sellingPrice2"), Double), G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("sellingPrice")) Then
                            'strX = Format(CType(row("sellingPrice"), Double), strPriceFormat)
                            strX = CType(row("sellingPrice"), Double).ToString(strPriceFormat, userCulture)
                        Else
                            strX = "---"
                        End If
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(7) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Imposed Price
                If blnIncludeImposedPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("ImposedPrice2")) Then

                            Dim dblAmount As Double = CDblDB(row("ImposedPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            'strX = Format(CType(dblAmount, Double), strPriceFormat) 'KMQDC 2016.09.14
                            strX = dblAmount.ToString(strPriceFormat, userCulture)
                            'strX = Format(CType(row("ImposedPrice2"), Double) + 0, G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("ImposedPrice")) Then
                            'strX = Format(CType(row("ImposedPrice"), Double) + 0, strPriceFormat)
                            strX = (CType(row("ImposedPrice"), Double) + 0).ToString(strPriceFormat, userCulture)
                        Else
                            strX = "---"
                        End If
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(8) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                '--- PRINT ROW ----
                If intCurrentY = 16 Then intCurrentY = 22 '// DRR 03.30.2012

                'Heading Per Category  (sorting) 
                If G_ReportOptions.strSortBy = "CategoryName" Then
                    Dim strHead = CType(row("CategoryName"), String)
                    If UCase(strGroupHeader) <> UCase(strHead) Then
                        strGroupHeader = strHead
                        blnNewGroup = True
                    End If
                End If

                If blnNewGroup Then
                    intCurrentY += 5
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroupHeader, fntDetail1, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                    intCurrentY += intTextHeight + 5
                    blnNewGroup = False
                End If

                'Category
                If blnIncludeCategory Then
                    strX = Trim(CType(row("CategoryName"), String))
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                End If
                intCurrentX += L_lngCol(1)

                'SubName
                If blnIncludeSubName Then
                    strX = Trim(CType(row("SubName"), String))
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngCol(2) - intColumnSpace, sf, Me.Padding).Height
                    '.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngSubName - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})

                    If G_ReportOptions.blShrinkToFit Then
                        Do While Not intRowHeight >= intTextHeight
                            sgFontSizeTemp -= 1
                            fntDetail2 = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                        Loop
                        intTextHeight = intRowHeight
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngSubName - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    Else
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngSubName - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                    End If
                End If
                intCurrentX += L_lngSubName

                'Name
                If blnIncludeName Then
                    strX = Trim(CType(row("Name"), String))
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngCol(2) - intColumnSpace, sf, Me.Padding).Height
                    '  .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngName - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})

                    If G_ReportOptions.blShrinkToFit Then
                        Do While Not intRowHeight >= intTextHeight
                            sgFontSizeTemp -= 1
                            fntDetail2 = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                        Loop
                        intTextHeight = intRowHeight
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngName - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                    Else
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngName - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                    End If
                End If
                intCurrentX += L_lngName

                If blnIncludeNumber Then
                    strX = Replace(row("Number").ToString, Chr(1), "")
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                End If
                intCurrentX += L_lngCol(2)

                'Cost Of Goods
                If blnIncludeCostOfGoods Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("calcprice2")) Then
                            Dim dblAmount As Double = CDblDB(row("calcprice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            'strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                            strX = (CType(dblAmount, Double) + 0).ToString(strPriceFormat, userCulture)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("calcprice")) Then
                            'strX = Format(CType(row("calcprice"), Double) + 0, strPriceFormat)
                            strX = (CType(row("calcprice"), Double) + 0).ToString(strPriceFormat, userCulture)
                        Else
                            strX = "---"
                        End If
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(3)

                'Date
                If blnIncludeDate Then
                    If Not IsDBNull(row("Dates")) Then
                        'strX = fctConvertDate(CType(row("Dates"), Date))
                        'strX = DirectCast(row("Dates"), DateTime).ToShortDateString
                        strX = DirectCast(row("Dates"), DateTime).ToString("d", userCulture)
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace + 10, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    End If
                End If
                intCurrentX += L_lngCol(4) + 10

                'Factor
                If blnIncludeFactor Then
                    If Not IsDBNull(row("coeff")) Then
                        strX = Format(fctConvertCoeff(CType(row("coeff"), Double), G_Options.FactorType), G_FormatTwoDecimal)
                    Else
                        strX = "---"
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(5)

                'Tax
                If blnIncludeTax Then
                    If Not IsDBNull(row("Tax")) Then
                        strX = Format(row("Tax"), G_FormatOneDecimal) + 0 & "%"
                    Else
                        strX = "---"
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(6)

                'Selling Price 
                If blnIncludeSellingPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("sellingPrice2")) Then
                            Dim dblAmount As Double = CDblDB(row("sellingPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            'strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                            strX = (CType(dblAmount, Double) + 0).ToString(strPriceFormat, userCulture)
                            'strX = Format(CType(row("sellingPrice2"), Double) + 0, G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("sellingPrice")) Then
                            'strX = Format(CType(row("sellingPrice"), Double) + 0, strPriceFormat)
                            strX = (CType(row("sellingPrice"), Double) + 0).ToString(strPriceFormat, userCulture)
                        Else
                            strX = "---"
                        End If
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(7)

                'Imposed Price
                If blnIncludeImposedPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("ImposedPrice2")) Then

                            Dim dblAmount As Double = CDblDB(row("ImposedPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            'strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                            strX = (CType(dblAmount, Double) + 0).ToString(strPriceFormat, userCulture)
                            'strX = Format(CType(row("ImposedPrice2"), Double) + 0, G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("ImposedPrice")) Then
                            'strX = Format(CType(row("ImposedPrice"), Double) + 0, strPriceFormat)
                            strX = (CType(row("ImposedPrice"), Double) + 0).ToString(strPriceFormat, userCulture)
                        Else
                            strX = "---"
                        End If
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If

                intCurrentY += GetLineSpace(intRowHeight)

            Next

        End With
        '--- PAGE DETAIL ----

        Me.PageFooter.Visible = False
        Return Me
    End Function


    Function PrintRecipeMenuListWithGross(ByVal dtRecipeList As DataTable, ByVal strReportTitle As String, _
                               ByVal strRecipeSubHeading As String, ByVal intPageLanguage As Integer, _
                               ByVal blnIncludeNumber As Boolean, ByVal blnIncludeCostOfGoods As Boolean, ByVal blnIncludeGross As Boolean, _
                               ByVal blnIncludeFactor As Boolean, ByVal blnIncludeTax As Boolean, _
                               ByVal blnIncludeSellingPrice As Boolean, ByVal blnIncludeImposedPrice As Boolean, _
                               ByVal blnIncludeDate As Boolean, ByVal strFontName As String, ByVal sgFontSize As Single, _
                               ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, _
                               ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double, _
                               ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, _
                               Optional ByVal blLandscape As Boolean = False, _
                               Optional ByVal strFontTitleName As String = "Arial", _
                               Optional ByVal sgFontTitleSize As Single = 16) As XtraReport


        Log.Info("umpisa")
        If Not dtRecipeList.Rows.Count > 0 Then Return Nothing
        Dim strX As String
        Dim cLang As New clsEGSLanguage(intPageLanguage)
        Cursor.Current = Cursors.WaitCursor
        sf = New StringFormat(StringFormatFlags.NoClip)

        '--- SETTINGS ---
        Dim fntTitle1 As Font = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold) 'Title
        Dim fntTitle2 As Font = New Font(strFontName, sgFontSize, FontStyle.Regular) 'Sub Title
        Dim fntTitle3 As Font = New Font(strFontName, sgFontSize, FontStyle.Bold) 'Column Title
        Dim fntDetail1 As Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
        Dim fntDetail2 As Font = New Font(strFontName, sgFontSize, FontStyle.Regular)
        Dim fntFooter1 As Font = New Font(strFontName, 8, FontStyle.Bold)

        Dim lblsubtitle As String = ""
        Dim lblfooter As String = ""

        Try
            lblsubtitle = dtRecipeList.Rows(0)("Subtitle").ToString()
            lblfooter = dtRecipeList.Rows(0)("Footer").ToString()
        Catch ex As Exception

        End Try


        With Me 'paper , orientation, width
            '.DataMember = dtRecipeList.TableName.ToString
            '.DataSource = dtRecipeList

            .PaperKind = Printing.PaperKind.Custom
            .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

            .Margins.Left = CInt(dblLeftMargin)
            .Margins.Top = CInt(dblTopMargin)
            .Margins.Bottom = CInt(dblBottomMargin)
            .Margins.Right = CInt(dblRightMargin)



            If blLandscape Then
                .Landscape = True
                intAvailableWidth = .PageHeight
            Else
                .Landscape = False
                intAvailableWidth = .PageWidth
            End If



            intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin) - 15


        End With
        '--- SETTINGS ---

        '--- PAGE HEADER ----
        With Me
            intCurrentX = 0
            intCurrentY = 0

            '>>Report Title<<
            strX = strReportTitle
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle1, intAvailableWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle1, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})
            intCurrentY = intTextHeight + 5

            '>>SubReport Header<<
            strX = lblsubtitle '"Sample recipes from April 1, 2016 - April 7, 2016" 'LD20160507 Sample
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle2, intAvailableWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle2, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentY += intTextHeight + 5

            '>>SubReport Header<<
            strX = strRecipeSubHeading
            intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle2, intAvailableWidth, sf, Me.Padding).Height
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle2, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentY += intTextHeight + 10
            .lblcopyright.Text = lblfooter '"Content is copyright c Vita-Mixx Corporation. All rights reserved."


            '>>Line<<
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
            intCurrentY += 10


            .lblcopyright.Visible = True
            .xrLineReport.Visible = True

            '>>Column Header<<
            sf = New StringFormat(StringFormatFlags.DirectionVertical)

            'Measure Column Width
            strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
            L_lngCol(1) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
            L_lngCol(2) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.GrossMargin)
            L_lngCol(3) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Grossmargininpercent)
            L_lngCol(4) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Const)
            L_lngCol(5) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
            L_lngCol(6) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
            L_lngCol(7) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
            L_lngCol(8) = ReportingTextUtils.MeasureText(fctGetLongerSubstring(strX), fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace

            strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
            L_lngCol(9) = ReportingTextUtils.MeasureText(strX, fntTitle3, intTextHeight, sf, Me.Padding).Height + intColumnSpace



            Dim strLastCurrency As String = Nothing
            Dim blnOneCurrency As Boolean = True
            For Each row As DataRowView In dtRecipeList.DefaultView

                Dim strPriceFormat As String = CType(row("CurFormat"), String)


                strX = Replace(CStrDB(row("Number")), Chr(1), "")
                intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If L_lngCol(1) < intTextHeight Then L_lngCol(1) = intTextHeight

                'Cost of Goods
                If G_ReportOptions.intYieldOption = 0 Then 'Per
                    If Not IsDBNull(row("calcprice2")) Then

                        Dim dblAmount As Double = CDblDB(row("calcprice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                        strX = Format(CType(dblAmount, Double), strPriceFormat) 'KMQDC 2016.09.14
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        If L_lngCol(2) < intTextHeight Then L_lngCol(2) = intTextHeight
                    End If
                Else 'Total
                    If Not IsDBNull(row("calcprice")) Then
                        strX = Format(CType(row("calcprice"), Double), strPriceFormat)
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        If L_lngCol(2) < intTextHeight Then L_lngCol(2) = intTextHeight
                    End If
                End If

                'Factor
                If Not IsDBNull(row("coeff")) Then
                    strX = Format(fctConvertCoeff(CType(row("coeff"), Double), G_Options.FactorType), G_FormatTwoDecimal)
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    If L_lngCol(5) < intTextHeight Then L_lngCol(5) = intTextHeight
                End If

                'Tax
                If Not IsDBNull(row("Tax")) Then
                    strX = Format(row("Tax"), G_FormatOneDecimal) & "%"
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    If L_lngCol(6) < intTextHeight Then L_lngCol(6) = intTextHeight
                End If

                'Selling Price
                If G_ReportOptions.intYieldOption = 0 Then 'Per
                    If Not IsDBNull(row("sellingprice2")) Then
                        Dim dblAmount As Double = CDblDB(row("sellingprice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                        strX = Format(CType(dblAmount, Double), strPriceFormat) 'KMQDC 2016.09.14
                        'strX = Format(CType(row("sellingprice2"), Double), G_strPriceFormat)
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        If L_lngCol(7) < intTextHeight Then L_lngCol(7) = intTextHeight
                    End If
                Else 'Total
                    If Not IsDBNull(row("sellingprice")) Then
                        strX = Format(CType(row("sellingprice"), Double), strPriceFormat)
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        If L_lngCol(7) < intTextHeight Then L_lngCol(7) = intTextHeight
                    End If
                End If

                'Imposed Price
                If G_ReportOptions.intYieldOption = 0 Then 'Per
                    If Not IsDBNull(row("ImposedPrice2")) Then
                        Dim dblAmount As Double = CDblDB(row("ImposedPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                        strX = Format(CType(dblAmount, Double), strPriceFormat) 'KMQDC 2016.09.14
                        'strX = Format(CType(row("ImposedPrice2"), Double), G_strPriceFormat)
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        If L_lngCol(8) < intTextHeight Then L_lngCol(8) = intTextHeight
                    End If
                Else 'Total
                    If Not IsDBNull(row("ImposedPrice")) Then
                        strX = Format(CType(row("ImposedPrice"), Double), strPriceFormat)
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                        If L_lngCol(8) < intTextHeight Then L_lngCol(8) = intTextHeight
                    End If
                End If

                'Date
                If Not IsDBNull(row("Dates")) Then
                    ' strX = fctConvertDate(CType(row("Dates"), Date))
                    strX = DirectCast(row("Dates"), DateTime).ToShortDateString 'JBQL format date automatically getting the culture info
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                    If L_lngCol(9) < intTextHeight Then L_lngCol(9) = intTextHeight
                End If

                'Currency
                ''strLastCurrency = CStrDB(row("Currency"))
                If CStrDB(row("Currency")) <> "" Then
                    If strLastCurrency <> row("Currency").ToString Then
                        blnOneCurrency = False
                        strLastCurrency = CStr(row("Currency"))
                    End If
                End If
                strX = strLastCurrency
                intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, intTextHeight, sf, Me.Padding).Height + intColumnSpace
                If L_lngCol(8) < intTextHeight Then L_lngCol(8) = intTextHeight
            Next
            If blnOneCurrency Then L_lngCol(8) = 0

            If Not blnIncludeNumber Then L_lngCol(1) = 0
            If Not blnIncludeCostOfGoods Then L_lngCol(2) = 0
            If Not blnIncludeGross Then L_lngCol(3) = 0
            If Not blnIncludeGross Then L_lngCol(4) = 0
            If Not blnIncludeFactor Then L_lngCol(5) = 0
            If Not blnIncludeTax Then L_lngCol(6) = 0
            If Not blnIncludeSellingPrice Then L_lngCol(7) = 0
            If Not blnIncludeImposedPrice Then L_lngCol(8) = 0
            If Not blnIncludeDate And Not blnIncludeSellingPrice And Not blnIncludeImposedPrice Then L_lngCol(9) = 0


            'PJRB 2017.03.10 ----- DISTRIBUTION OF COLUMN WIDTH 
            'ONLY WHEN sum of all column width exceeds available width 
            'subtract width on these columns: Number, Cost of Goods, Factor, Selling Price, and Imposed Selling Price Column width

            Dim widthForName As Integer = 100      'EDIT HERE (MINIMUM WIDTH FOR COLUMN 'NAME')
            Dim minWidth As Integer = 60           'EDIT HERE (MINIMUM WIDTH FOR OTHER COLUMNS STATED ABOVE)

            If intAvailableWidth < (L_lngCol(1) + L_lngCol(2) + L_lngCol(3) + L_lngCol(4) + L_lngCol(5) + L_lngCol(6) + L_lngCol(7) + L_lngCol(8) + L_lngCol(9) + widthForName) Then
                Dim tmpSub As Integer = (L_lngCol(1) + L_lngCol(2) + L_lngCol(3) + L_lngCol(4) + L_lngCol(5) + L_lngCol(6) + L_lngCol(7) + L_lngCol(8) + L_lngCol(9) + widthForName) - (intAvailableWidth)

                Dim countDivisor As Integer = 0
                Dim excessWidth As Integer = 0
                Dim LowerMinimum As New ArrayList
                Dim AboveMinimum As New ArrayList

                'count columns that has width > 0
                For Each col In L_lngCol
                    If col > 0 Then
                        countDivisor += 1
                    End If
                Next

                If countDivisor > 7 Then countDivisor = 7
                If countDivisor < 1 Then countDivisor = 1

                'width to be subtracted
                tmpSub /= countDivisor

                countDivisor = 7

                'subraction width of columns (cut subtraction if lower than minimum)
                If blnIncludeNumber Then 'Number
                    L_lngCol(1) -= tmpSub
                    If L_lngCol(1) < minWidth Then
                        excessWidth += minWidth - L_lngCol(1)
                        L_lngCol(1) = minWidth
                        LowerMinimum.Add(1)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(1)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeCostOfGoods Then 'Cost of Goods
                    L_lngCol(2) -= tmpSub
                    If L_lngCol(2) < minWidth Then
                        excessWidth += minWidth - L_lngCol(2)
                        L_lngCol(2) = minWidth
                        LowerMinimum.Add(2)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(2)
                    End If
                Else
                    countDivisor -= 1
                End If


                'If blnIncludeGross Then 'gross margin
                '    L_lngCol(3) -= tmpSub
                '    If L_lngCol(3) < minWidth Then
                '        excessWidth += minWidth - L_lngCol(3)
                '        L_lngCol(3) = minWidth
                '        LowerMinimum.Add(3)
                '        countDivisor -= 1
                '    Else
                '        AboveMinimum.Add(3)
                '    End If
                'Else
                '    countDivisor -= 1
                'End If

                'If blnIncludeGross Then 'gross margin
                '    L_lngCol(4) -= tmpSub
                '    If L_lngCol(4) < minWidth Then
                '        excessWidth += minWidth - L_lngCol(4)
                '        L_lngCol(4) = minWidth
                '        LowerMinimum.Add(4)
                '        countDivisor -= 1
                '    Else
                '        AboveMinimum.Add(4)
                '    End If
                'Else
                '    countDivisor -= 1
                'End If

                If blnIncludeFactor Then 'Factor
                    L_lngCol(5) -= tmpSub
                    If L_lngCol(5) < minWidth Then
                        excessWidth += minWidth - L_lngCol(5)
                        L_lngCol(5) = minWidth
                        LowerMinimum.Add(5)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(5)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeSellingPrice Then 'Selling Price
                    L_lngCol(7) -= tmpSub
                    If L_lngCol(7) < minWidth Then
                        excessWidth += minWidth - L_lngCol(7)
                        L_lngCol(7) = minWidth
                        LowerMinimum.Add(7)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(7)
                    End If
                Else
                    countDivisor -= 1
                End If

                If blnIncludeImposedPrice Then 'Imposed Selling Price
                    L_lngCol(8) -= tmpSub
                    If L_lngCol(8) < minWidth Then
                        excessWidth += minWidth - L_lngCol(8)
                        L_lngCol(8) = minWidth
                        LowerMinimum.Add(8)
                        countDivisor -= 1
                    Else
                        AboveMinimum.Add(8)
                    End If
                Else
                    countDivisor -= 1
                End If


                'if there is/are columns lower than minimum after subtraction
                If LowerMinimum.Count > 0 Then

                    Dim tmpWidth2 As Integer = Math.Ceiling(excessWidth / countDivisor)
                    Dim dblComputedValue() As Double = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                    Dim excessWidth2 As Integer = 0
                    Dim colCount As Integer = 0

                    For Each col In AboveMinimum
                        dblComputedValue(col) = L_lngCol(col)
                    Next

                    'check all columns that are capable to subract width again
                    While colCount <> countDivisor
                        For c = 0 To dblComputedValue.Count - 1
                            If dblComputedValue(c) > 0 Then
                                If dblComputedValue(c) - tmpWidth2 < minWidth Then
                                    dblComputedValue(c) = 0
                                Else
                                    colCount += 1
                                End If
                            End If
                        Next

                        If colCount <> countDivisor Then
                            tmpWidth2 = excessWidth / countDivisor
                            colCount = 0
                            countDivisor -= 1
                        End If

                    End While
                    Log.Info("bragadigdigadasdasdasdasdasdasd11111111111")

                    'subtract width of columns
                    If colCount > 0 Then
                        tmpWidth2 = Math.Ceiling(excessWidth / countDivisor)
                        For c = 0 To dblComputedValue.Count - 1
                            If dblComputedValue(c) > 0 Then
                                L_lngCol(c) -= tmpWidth2
                            End If
                        Next
                    End If
                End If
            End If
            'PJRB 2017.03.10 ----- END DISTRIBUTION OF COLUMN WIDTH

            L_lngNameW = Math.Abs(intAvailableWidth - L_lngCol(1) - L_lngCol(2) - L_lngCol(3) - L_lngCol(4) - L_lngCol(5) - L_lngCol(6) - L_lngCol(7) - L_lngCol(8) - L_lngCol(9))


            'Measure Column Header Height
            sf = New StringFormat(StringFormatFlags.NoClip)
            intTextHeight = 0

            'Number
            If blnIncludeNumber Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                intTextHeight = ReportingTextUtils.MeasureText(strX, fntTitle3, L_lngCol(1) - intColumnSpace, sf, Me.Padding).Height
            End If

            'Recipe Name
            strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
            intTextHeight = fctGetHighest(strX, intTextHeight, L_lngNameW - intColumnSpace, fntTitle3, sf)

            'Cost Of Goods
            Dim strCurrencyHeading As String = ""
            If strLastCurrency <> "" Then strCurrencyHeading = vbCrLf & strLastCurrency
            If blnIncludeCostOfGoods Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods) & strCurrencyHeading
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(2) - intColumnSpace, fntTitle3, sf)
            End If

            If blnIncludeGross Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.GrossMargin)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(3) - intColumnSpace, fntTitle3, sf)
            End If

            If blnIncludeGross Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Grossmargininpercent)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(4) - intColumnSpace, fntTitle3, sf)
            End If

            'Factor
            If blnIncludeFactor Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Const)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(5) - intColumnSpace, fntTitle3, sf)
            End If

            'Tax
            If blnIncludeTax Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(6) - intColumnSpace + 10, fntTitle3, sf)
            End If

            'Selling Price
            If blnIncludeSellingPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.SellingPricePlusTax) & vbCrLf & strCurrencyHeading
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(7) - intColumnSpace, fntTitle3, sf)
            End If

            If blnIncludeImposedPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(8) - intColumnSpace, fntTitle3, sf)
            End If

            If blnIncludeDate Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
                intTextHeight = fctGetHighest(strX, intTextHeight, L_lngCol(9) - intColumnSpace, fntTitle3, sf)
            End If


            'Print Column Header
            If blnIncludeNumber Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Number)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            End If
            intCurrentX += L_lngCol(1)

            strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe)
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngNameW + 10, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

            intCurrentX += L_lngNameW + 10

            If blnIncludeCostOfGoods Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & strCurrencyHeading, fntTitle3, intCurrentX, intCurrentY, L_lngCol(2), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
            End If
            intCurrentX += L_lngCol(2)

            If blnIncludeGross Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.GrossMargin)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(3), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If
            intCurrentX += L_lngCol(3)

            If blnIncludeGross Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Grossmargininpercent)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(4), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If
            intCurrentX += L_lngCol(4)

            If blnIncludeFactor Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Const)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(5), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})

            End If
            intCurrentX += L_lngCol(5)

            If blnIncludeTax Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(6), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight, True)})
            End If
            intCurrentX += L_lngCol(6)


            If blnIncludeSellingPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & " (" & strCurrencyHeading & ")", fntTitle3, intCurrentX, intCurrentY, L_lngCol(7), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If
            intCurrentX += L_lngCol(7)

            If blnIncludeImposedPrice Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX & " (" & strCurrencyHeading & ")", fntTitle3, intCurrentX, intCurrentY, L_lngCol(8), intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If
            intCurrentX += L_lngCol(8)


            If blnIncludeDate Then
                strX = cLang.GetString(clsEGSLanguage.CodeType.Date_)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntTitle3, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
            End If

            intCurrentX = 0
            intCurrentY += intTextHeight
            .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 3)})

        End With
        '--- PAGE HEADER ----

        Log.Info("bragadigdigadasdasdasdasdasdasd222222222222")
        '--- PAGE DETAIL ----
        With Me
            intCurrentX = 0
            ''-- JBB 04.12.2011
            ''-- Orig
            'intCurrentY = 17 
            ''-- Edit
            intCurrentY = 0
            .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel("", fntDetail1, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            intCurrentY += GetLineSpace(intTextHeight)
            ''---
            sf = New StringFormat(StringFormatFlags.MeasureTrailingSpaces)

            Dim dvRecipeList As New DataView(dtRecipeList)
            dvRecipeList.Sort = G_ReportOptions.strSortBy
            dtRecipeList = dvRecipeList.ToTable

            Dim intRowHeight As Integer = 0
            Dim sgFontSizeTemp As Single
            Dim strGroupHeader As String = ""
            Dim blnNewGroup As Boolean = False

            For Each row As DataRowView In dtRecipeList.DefaultView
                intCurrentX = 0
                sgFontSizeTemp = sgFontSize

                Dim strPriceFormat As String = CType(row("CurFormat"), String)
                'MEASURE ROW HEIGHT ====
                'Number
                If blnIncludeNumber Then
                    strX = Replace(row("Number").ToString, Chr(1), "")
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(1) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Name
                strX = Trim(CType(row("Name"), String))
                intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNameW - intColumnSpace, sf, Me.Padding).Height
                If G_ReportOptions.blShrinkToFit Then
                    Do While Not intRowHeight >= intTextHeight
                        sgFontSizeTemp -= 1
                        fntDetail2 = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                    Loop
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                Else
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Cost Of Goods
                If blnIncludeCostOfGoods Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("calcprice2")) Then
                            Dim dblAmount As Double = CDblDB(row("calcprice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("calcprice")) Then
                            strX = Format(CType(row("calcprice"), Double) + 0, strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(2) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If
                If blnIncludeGross Then
                    If Not IsDBNull(row("GrossMargin")) Then
                        Dim grossmargin As Double = CDblDB(row("GrossMargin"))
                        strX = Format(CType(grossmargin, Double) + 0, strPriceFormat)
                    Else
                        strX = "---"
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(3) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                If blnIncludeGross Then
                    If Not IsDBNull(row("GrossMarginPercent")) Then
                        strX = Format(row("GrossMarginPercent"), G_FormatOneDecimal) + 0 & "%"
                    Else
                        strX = "---"
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(4) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Factor
                If blnIncludeFactor Then
                    If Not IsDBNull(row("coeff")) Then
                        strX = Format(fctConvertCoeff(CType(row("coeff"), Double), G_Options.FactorType), G_FormatTwoDecimal)
                    Else
                        strX = "---"
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(5) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Tax
                If blnIncludeTax Then
                    If Not IsDBNull(row("Tax")) Then
                        strX = Format(row("Tax"), G_FormatOneDecimal) + 0 & "%"
                    Else
                        strX = "---"
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(6) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Selling Price
                If blnIncludeSellingPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("sellingPrice2")) Then

                            Dim dblAmount As Double = CDblDB(row("sellingPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            strX = Format(CType(dblAmount, Double), strPriceFormat) 'KMQDC 2016.09.14
                            'strX = Format(CType(row("sellingPrice2"), Double), G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("sellingPrice")) Then
                            strX = Format(CType(row("sellingPrice"), Double), strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(7) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Imposed Price
                If blnIncludeImposedPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("ImposedPrice2")) Then

                            Dim dblAmount As Double = CDblDB(row("ImposedPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            strX = Format(CType(dblAmount, Double), strPriceFormat) 'KMQDC 2016.09.14
                            'strX = Format(CType(row("ImposedPrice2"), Double) + 0, G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("ImposedPrice")) Then
                            strX = Format(CType(row("ImposedPrice"), Double) + 0, strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    End If
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(8) - intColumnSpace, sf, Me.Padding).Height
                    If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                End If

                'Date
                If blnIncludeDate Then
                    If Not IsDBNull(row("Dates")) Then
                        'strX = fctConvertDate(CType(row("Dates"), Date))
                        strX = DirectCast(row("Dates"), DateTime).ToShortDateString 'JBQL format date automatically getting the culture info
                        intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail1, L_lngCol(9) - intColumnSpace, sf, Me.Padding).Height
                        If intRowHeight < intTextHeight Then intRowHeight = intTextHeight
                    End If
                End If

                'PRINT ROW ====
                'Number
                If intCurrentY = 16 Then intCurrentY = 22 '// DRR 03.30.2012

                'Heading Per Category  (sorting) 
                If G_ReportOptions.strSortBy = "CategoryName" Then
                    Dim strHead = CType(row("CategoryName"), String)
                    If UCase(strGroupHeader) <> UCase(strHead) Then
                        strGroupHeader = strHead
                        blnNewGroup = True
                    End If
                End If

                If blnNewGroup Then
                    intCurrentY += 5
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strGroupHeader, fntDetail1, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, , DevExpress.XtraPrinting.BorderSide.Bottom)})
                    intCurrentY += intTextHeight + 5
                    blnNewGroup = False
                End If

                If blnIncludeNumber Then
                    strX = Replace(row("Number").ToString, Chr(1), "")
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(1) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                End If
                intCurrentX += L_lngCol(1)

                'Name
                strX = Trim(CType(row("Name"), String))
                intTextHeight = ReportingTextUtils.MeasureText(strX, fntDetail2, L_lngNameW - intColumnSpace, sf, Me.Padding).Height

                If G_ReportOptions.blShrinkToFit Then
                    Do While Not intRowHeight >= intTextHeight
                        sgFontSizeTemp -= 1
                        fntDetail2 = New Font(strFontName, sgFontSizeTemp, FontStyle.Regular)
                    Loop
                    intTextHeight = intRowHeight
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngNameW + 10, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                Else
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail2, intCurrentX, intCurrentY, L_lngNameW + 10, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, , True)})
                End If
                intCurrentX += L_lngNameW + 10

                'Cost Of Goods
                If blnIncludeCostOfGoods Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("calcprice2")) Then
                            Dim dblAmount As Double = CDblDB(row("calcprice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("calcprice")) Then
                            strX = Format(CType(row("calcprice"), Double) + 0, strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(2) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(2)

                If blnIncludeGross Then
                    If Not IsDBNull(row("GrossMargin")) Then
                        Dim grossmargin As Double = CDblDB(row("GrossMargin"))
                        strX = Format(CType(grossmargin, Double) + 0, strPriceFormat)
                    Else
                        strX = "---"
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(3) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(3)

                If blnIncludeGross Then
                    If Not IsDBNull(row("GrossMarginPercent")) Then
                        strX = Format(row("GrossMarginPercent"), G_FormatOneDecimal) + 0 & "%"
                    Else
                        strX = "---"
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(4) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    intCurrentX += L_lngCol(4)
                End If

                'Factor
                If blnIncludeFactor Then
                    If Not IsDBNull(row("coeff")) Then
                        strX = Format(fctConvertCoeff(CType(row("coeff"), Double), G_Options.FactorType), G_FormatTwoDecimal)
                    Else
                        strX = "---"
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(5) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(5) - intColumnSpace

                'Tax
                If blnIncludeTax Then
                    If Not IsDBNull(row("Tax")) Then
                        strX = Format(row("Tax"), G_FormatOneDecimal) + 0 & "%"
                    Else
                        strX = "---"
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(6) - intColumnSpace + 10, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(6)

                'Selling Price 
                If blnIncludeSellingPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("sellingPrice2")) Then
                            Dim dblAmount As Double = CDblDB(row("sellingPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                            'strX = Format(CType(row("sellingPrice2"), Double) + 0, G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("sellingPrice")) Then
                            strX = Format(CType(row("sellingPrice"), Double) + 0, strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(7) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(7)

                'Imposed Price
                If blnIncludeImposedPrice Then
                    If G_ReportOptions.intYieldOption = 0 Then 'Per
                        If Not IsDBNull(row("ImposedPrice2")) Then

                            Dim dblAmount As Double = CDblDB(row("ImposedPrice2")) / CDblDB(row("yield")) 'KMQDC 2016.09.14
                            strX = Format(CType(dblAmount, Double) + 0, strPriceFormat) 'KMQDC 2016.09.14
                            'strX = Format(CType(row("ImposedPrice2"), Double) + 0, G_strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    Else 'Total
                        If Not IsDBNull(row("ImposedPrice")) Then
                            strX = Format(CType(row("ImposedPrice"), Double) + 0, strPriceFormat)
                        Else
                            strX = "---"
                        End If
                    End If
                    .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(8) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                End If
                intCurrentX += L_lngCol(8)

                'Date
                If blnIncludeDate Then
                    If Not IsDBNull(row("Dates")) Then
                        'strX = fctConvertDate(CType(row("Dates"), Date))
                        strX = DirectCast(row("Dates"), DateTime).ToShortDateString
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntDetail1, intCurrentX, intCurrentY, L_lngCol(9) - intColumnSpace, intRowHeight, DevExpress.XtraPrinting.TextAlignment.TopRight)})
                    End If
                End If

                intCurrentY += GetLineSpace(intRowHeight)
            Next
        End With
        '--- PAGE DETAIL ----
        Log.Info("bragadigdigadasdasdasdasdasdasd33333333333333")
        '--- PAGE FOOTER ---
        With Me
            .xrLblEGS.Font = fntFooter
            .xrLinePF.Left = 0
            '.xrLinePF.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})
            .xrLinePF.Width = intAvailableWidth
            .xrPIPageNumber.Left = intAvailableWidth / 2
            .xrPIPageNumber.Width = intAvailableWidth / 2
            .xrLblEGS.Left = 0
            .xrLblEGS.Width = intAvailableWidth / 2
            .xrLinePF.Visible = True
            .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"
            If NoPrintLines Then .xrLinePF.Visible = False
            subReportFooter(intAvailableWidth, strFontName)
        End With
        '--- PAGE FOOTER ---
        Log.Info("bragadigdigadasdasdasdasdasdasd444444444444")
        Return Me
    End Function
    Function fctPrintMerchandiseThumbnailsList(ByVal dtMerchandiseList As DataTable, ByVal strSubHeading As String, _
                                  ByVal intLanguage As Integer, _
                                  ByVal strFontName As String, ByVal sgFontSize As Single, _
                                  ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double, _
                                  ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, Optional ByVal blLandscape As Boolean = False, _
                                  Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16) As XtraReport

        Dim cLang As New clsEGSLanguage(intLanguage)
        Dim drvReport As DataRowView
        'Dim blnOneCurrency As Boolean
        'Dim dblWastage As Double

        'Dim strLastCurrency As String
        'Dim strX As String
        'Dim strSupplier As String

        Cursor.Current = Cursors.WaitCursor

        sf = New StringFormat(StringFormatFlags.NoClip)

        Try
            fntReportTitle = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold)
            fntRegular = New Font(strFontName, sgFontSize, FontStyle.Regular)
            fntBold = New Font(strFontName, sgFontSize, FontStyle.Bold)
            fntFooter = New Font(strFontName, 8, FontStyle.Bold)
        Catch ex As Exception
            Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
            Return Me
        End Try
        xrLblEGS.Font = fntFooter

        If dtMerchandiseList.DefaultView.Count > 0 Then
            With Me
                Me.DataMember = dtMerchandiseList.TableName.ToString
                Me.DataSource = dtMerchandiseList

                'Papersize
                '------------
                .PaperKind = Printing.PaperKind.Custom
                .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)

                'Margins
                '------------
                .Margins.Left = CInt(dblLeftMargin)
                .Margins.Top = CInt(dblTopMargin)
                .Margins.Bottom = CInt(dblBottomMargin)
                .Margins.Right = CInt(dblRightMargin)

                'Orientation
                '-----------
                If blLandscape Then
                    .Landscape = True
                    intAvailableWidth = .PageHeight
                Else
                    .Landscape = False
                    intAvailableWidth = .PageWidth
                End If

                intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin) - 30

                .xrLinePF.Left = 0
                .xrLinePF.Width = intAvailableWidth
                .xrPIPageNumber.Left = intAvailableWidth / 2
                .xrPIPageNumber.Width = intAvailableWidth / 2
                .xrPIPageNumber.Font = fntBold

                .xrLblEGS.Left = 0
                .xrLblEGS.Width = intAvailableWidth / 2
                .xrLblEGS.Font = fntBold
                .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

                If NoPrintLines Then
                    .xrLinePF.Visible = False
                End If
                subReportFooter(intAvailableWidth, strFontName)

                intCurrentX = 0
                intCurrentY = 0

                'Report Title
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.IngredientList)  'Merchandise List
                intTextHeight = ReportingTextUtils.MeasureText(strReportTitle, fntReportTitle, intAvailableWidth, sf, Me.Padding).Height
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strReportTitle, fntReportTitle, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, , , , , , G_ReportOptions.strTitleColor)})
                intCurrentY += intTextHeight

                'Sub Report Header
                intTextHeight = ReportingTextUtils.MeasureText(strSubHeading, fntRegular, intAvailableWidth, sf, Me.Padding).Height
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeXRLabel(strSubHeading, fntRegular, intCurrentX, intCurrentY, intAvailableWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

                intCurrentY += GetLineSpace(intTextHeight)
                .PageHeader.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, intCurrentY, intAvailableWidth, 1)})


                '******* measurements
                Dim dblMaxWidthPicture As Double = 90
                Dim dblMaxHeightPicture As Double = 90
                Dim intColumnWidth As Integer = (intAvailableWidth / 3)

                '******* Get Names ----
                Dim strNames As String = ""
                Dim strNamesTrans As String = ""
                Dim strPictures As String = ""
                For Each drvReport In dtMerchandiseList.DefaultView
                    '--- Assign Variables
                    Dim strName As String = drvReport("Name")
                    Dim strName2 As String = IIf(IsDBNull(drvReport("NameEnglish")), "", drvReport("NameEnglish"))   'drvReport("NameTrans")
                    Dim arrPictures As String = drvReport("pictureName")

                    Dim arrPicture() As String = arrPictures.Split(CChar(";"))

                    strNames += strName & ";"
                    strNamesTrans += strName2 & ";"
                    For i As Integer = 0 To UBound(arrPicture)
                        If fctFileExists(G_strPhotoPath & arrPicture(i)) And arrPicture(i) <> "" Then
                            strPictures += arrPicture(i) & ";"
                            'strFileName &= arrPicture(i)
                            Exit For
                        Else
                            If i = UBound(arrPicture) Then
                                strPictures += ";"
                            End If
                        End If
                    Next
                Next


                Dim arrFetchName() As String = strNames.Trim.Split(CChar(";"))
                Dim arrFetchNameTrans() As String = strNamesTrans.Trim.Split(CChar(";"))
                Dim arrFetchPicture() As String = strPictures.Trim.Split(CChar(";"))



                '*** Print
                intCurrentX = 0
                intCurrentY = 0
                Dim strFileName As String = G_strPhotoPath

                Dim flagNotOk As Boolean
                Dim intCountArr As Integer = 0
                Dim intPastY As Integer = 0
                sf = New StringFormat(StringFormatFlags.NoClip)
                intTextHeight = (ReportingTextUtils.MeasureText("A", fntRegular, intColumnWidth, sf, Me.Padding).Height) * 2
SetCountToZero:
                intPastY += intCurrentY
                intCurrentY = intPastY
                intCurrentX = 0
                Dim intCol As Integer
                For intCol = 0 To 2
                    If intCountArr = dtMerchandiseList.Rows.Count Then
                        Exit For
                    Else
                        flagNotOk = True
                        If Not fctFileExists(G_strPhotoPath + arrFetchPicture(intCountArr)) Then
                            flagNotOk = False
                            GoTo Next_record
                        End If
                        If Not arrFetchPicture(intCountArr) = "" Then
                            .Detail.Controls.AddRange(New XRControl() {fctMakePictureBox(strFileName + arrFetchPicture(intCountArr), intCurrentX, intCurrentY, CInt(dblMaxWidthPicture), CInt(dblMaxHeightPicture))})
                        End If
                        intCurrentY = intPastY + dblMaxHeightPicture + 2
                        If arrFetchName(intCountArr).ToString <> arrFetchNameTrans(intCountArr).ToString Then
                            arrFetchName(intCountArr) = arrFetchName(intCountArr).ToString & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & arrFetchNameTrans(intCountArr).ToString
                        End If
                        .Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(arrFetchName(intCountArr).ToString, fntRegular, intCurrentX, intCurrentY, intColumnWidth - 2, intTextHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
                        intCurrentX += intColumnWidth
                    End If

Next_record:

                    If flagNotOk = False Then
                        If intCol = 0 Then
                            intCol = -1
                        Else
                            intCol = intCol - 1
                        End If


                        intCountArr += 1
                    Else
                        intCurrentY = intPastY
                        intCountArr += 1
                        If intCol = 2 Then
                            intCurrentY = dblMaxHeightPicture + 2 + (intTextHeight * 2)
                            GoTo SetCountToZero
                        End If
                    End If
                Next
                dtMerchandiseList.Reset()
            End With
        End If
        Return Me

    End Function
#End Region

    '-------------------------------------------------------------'
    'MCM 02.01.06 
    '-------------------------------------------------------------'
    ''Function fctMasterReport(ByVal dtToPrint As DataTable, _
    ''                                    ByVal dblPageWidth As Double, ByVal dblPageHeight As Double, _
    ''                                    ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double, _
    ''                                    ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, _
    ''                                    ByVal strFontName As String, ByVal sgFontSize As Single, _
    ''                                    Optional ByVal blLandscape As Boolean = False, _
    ''                                    Optional ByVal printType As enumReportType = enumReportType.None, _
    ''                                    Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16) As XtraReport 'VRP 05.11.2007

    ''    Dim cLang As New clsEGSLanguage(G_ReportOptions.intPageLanguage)
    ''    Dim intAvailableWidth2 As Integer

    ''    Try
    ''        fntBold = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold)
    ''        fntFooter = New Font(strFontName, sgFontSize, FontStyle.Bold)
    ''    Catch ex As Exception
    ''        Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
    ''        Return Me
    ''    End Try

    ''    xrLblEGS.Font = fntFooter

    ''    fctMasterReport = Nothing

    ''    With Me
    ''        Select Case G_ReportOptions.dblReportType
    ''            Case 1, 2, 3, 6  'Recipe Standard,Modern, costing
    ''                If Not dtToPrint.Columns.Contains("ChildLevel") Then dtToPrint.Columns.Add(New DataColumn("ChildLevel", System.Type.GetType("System.Int16")))

    ''                Dim rw As DataRow
    ''                For Each rw In dtToPrint.Rows
    ''                    'If IsDBNull(rw("CodeListeParent")) Then rw("CodeListeParent") = "0"
    ''                    rw("ChildLevel") = ChildLevel(rw, dtToPrint)
    ''                Next


    ''                'dtToPrint.DefaultView.Sort = "CodeListeMain,ChildLevel"
    ''        End Select
    ''        .DataMember = dtToPrint.TableName.ToString
    ''        .DataSource = dtToPrint

    ''        .PaperKind = Printing.PaperKind.Custom
    ''        .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)
    ''        'Margins
    ''        '------------
    ''        .Margins.Left = 0 'CInt(dblLeftMargin)
    ''        .Margins.Top = 0 'CInt(dblTopMargin)
    ''        .Margins.Bottom = 0 'CInt(dblBottomMargin)
    ''        .Margins.Right = 0 'CInt(dblRightMargin)
    ''        '.PageHeader.Height = CInt(dblTopMargin)
    ''        'Orientation
    ''        '-----------
    ''        If blLandscape Then
    ''            .Landscape = True
    ''            intAvailableWidth = CInt(dblPageHeight)
    ''        Else
    ''            .Landscape = False
    ''            intAvailableWidth = CInt(dblPageWidth)
    ''        End If
    ''        intAvailableWidth2 = intAvailableWidth

    ''        'Select Case DisplayRecipeDetails 'VRP 06.08.2008 
    ''        '    Case 2 'ADF
    ''        '        With Me 'paper , orientation, width
    ''        '            .PaperKind = New System.Drawing.Printing.PaperKind
    ''        '            .PaperKind = Printing.PaperKind.A4
    ''        '            .Margins = New System.Drawing.Printing.Margins(50.0!, 50.0!, 50.0!, 50.0!)
    ''        '            .Landscape = False
    ''        '            intAvailableWidth = (.PageWidth - .Margins.Left - .Margins.Right)
    ''        '            intAvailableHeight = (.PageHeight - .Margins.Top - .Margins.Bottom)
    ''        '        End With
    ''        '    Case Else
    ''        '        With Me
    ''        '            .PaperKind = Printing.PaperKind.Custom
    ''        '            .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)
    ''        '            .Margins = New System.Drawing.Printing.Margins(0.0!, 0.0!, 0.0!, 0.0!)
    ''        '            If blLandscape Then
    ''        '                .Landscape = True
    ''        '                intAvailableWidth = CInt(dblPageHeight)
    ''        '            Else
    ''        '                .Landscape = False
    ''        '                intAvailableWidth = CInt(dblPageWidth)
    ''        '            End If
    ''        '            intAvailableWidth2 = intAvailableWidth
    ''        '        End With
    ''        'End Select

    ''        intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin)
    ''        Select Case G_ReportOptions.dblReportType
    ''            Case 1, 2, 3  'Recipe Standard,Modern
    ''                'DetailReport.fctPrintRecipeDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont)
    ''                intAvailableWidth2 = intAvailableWidth2 - 15
    ''                intAvailableWidth = intAvailableWidth - 15
    ''            Case 4 'Layout
    ''                'DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
    ''            Case 5  'Merchandise Details
    ''                'DetailReport.fctPrintMerchandiseEgsDetails(foundRecipeDetails, foundKeywords, foundAllergens)
    ''                intAvailableWidth2 = intAvailableWidth2 - 30
    ''                intAvailableWidth = intAvailableWidth - 30
    ''            Case 6  'Recipe Costing
    ''                'DetailReport.fctPrintRecipeCostingEGSStandard(foundRecipeDetails, foundKeywords, foundAllergens)
    ''            Case 7 'mcm 12.01.05 Menu Costing
    ''                'DetailReport.fctPrintMenuCostingEGSStandard(foundRecipeDetails, foundKeywords, foundAllergens)
    ''            Case 21, 22, 23
    ''                'DetailReport.fctPrintMenuDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont)
    ''            Case 24
    ''                'DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
    ''        End Select



    ''        .xrLinePF.Left = dblLeftMargin
    ''        .xrLinePF.Width = intAvailableWidth

    ''        .xrLblEGS.Left = dblLeftMargin
    ''        .xrLblEGS.Width = Int(intAvailableWidth / 2)

    ''        .xrPIPageNumber.Left = .xrLblEGS.Width + dblLeftMargin
    ''        .xrPIPageNumber.Width = Int(intAvailableWidth / 2)
    ''        .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

    ''        If NoPrintLines Then
    ''            .xrLinePF.Visible = False
    ''        End If

    ''        '---- Report Footer Logo and Address ------ 'DLS August282007
    ''        subReportFooter(intAvailableWidth2 - CInt(dblRightMargin), strFontName)
    ''        'If G_ReportOptions.strFooterLogoPath <> "" Then
    ''        '    .XrFooterPicLogo.Image = Image.FromFile(G_ReportOptions.strFooterLogoPath)
    ''        '    .XrFooterPicLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.Normal
    ''        '    .XrFooterPicLogo.Size = New System.Drawing.Size(156, 48)
    ''        '    .XrFooterPicLogo.Location = New System.Drawing.Point(intAvailableWidth2 - CInt(dblRightMargin) - 156, 8)

    ''        '    .xrLblEGS.Text = G_ReportOptions.strFooterAddress
    ''        '    .xrLblEGS.Width = (intAvailableWidth - 10)
    ''        '    .xrLblEGS.Height = 50
    ''        '    .xrLblEGS.Multiline = True
    ''        '    .xrLblEGS.Size = New System.Drawing.Size(442, 50)
    ''        '    .xrLblEGS.Text = G_ReportOptions.strFooterAddress 'Replace(G_ReportOptions.strFooterAddress, vbCrLf, Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
    ''        '    .PageFooter.Height = 80
    ''        'Else
    ''        '    .XrFooterPicLogo.Visible = False
    ''        '    .xrLblEGS.Location = New System.Drawing.Point(0, 8)
    ''        '    .xrLblEGS.Text = ""
    ''        '    .PageFooter.Height = 30
    ''        'End If


    ''        .Detail.Controls.AddRange(New XRControl() {lblID})
    ''        lblID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "Code")
    ''        'lblID.Visible = True

    ''        Select Case G_ReportOptions.dblReportType
    ''            Case 1, 2, 3, 6  'Recipe Standard,Modern, costing
    ''                lblMain.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeListeMain")
    ''                'lblMain.Visible = True
    ''                lblParent.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeListeParent")
    ''                lblParent.Visible = True
    ''        End Select

    ''        .SubRptDetail.Location = New System.Drawing.Point(dblLeftMargin, 0) 'dblTopMargin
    ''        SubRptDetail.Visible = True
    ''    End With
    ''    Return Me
    ''End Function



    Dim CodePrintList As Integer
    Dim dtFoundDetail As DataTable
    Dim dtFootNotes As DataTable
    Dim dtStep As DataTable
    Dim dtListeNote As DataTable
    'Commented Original fctMasterReport
    'VRP 06.08.2008 
    Function fctMasterReport(ByVal dtToPrint As DataTable,
                                        ByVal dblPageWidth As Double, ByVal dblPageHeight As Double,
                                        ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double,
                                        ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double,
                                        ByVal strFontName As String, ByVal sgFontSize As Single,
                                        Optional ByVal blLandscape As Boolean = False,
                                        Optional ByVal printType As enumReportType = enumReportType.None,
                                        Optional ByVal strFontTitleName As String = "Arial", Optional ByVal sgFontTitleSize As Single = 16,
                                        Optional ByVal intCodePrintList As Integer = 0, Optional dtDetails2 As DataTable = Nothing,
                                        Optional dtFootNote As DataTable = Nothing, Optional dtSteps As DataTable = Nothing,
                                        Optional dtListeNotes As DataTable = Nothing,
                             Optional ByVal userLocale As String = "en-US") As XtraReport 'VRP 05.11.2007

        Try
            Dim cLang As New clsEGSLanguage(G_ReportOptions.intPageLanguage)
            Dim intAvailableWidth2 As Integer
            strUserLocale = userLocale
            dtFoundDetail = dtDetails2
            dtFootNotes = dtFootNote
            dtListeNote = dtListeNotes
            dtStep = dtSteps
            Try
                fntBold = New Font(strFontTitleName, sgFontTitleSize, FontStyle.Bold)
                fntFooter = New Font(strFontName, sgFontSize, FontStyle.Bold)
            Catch ex As Exception
                Me.Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(cLang.GetString(clsEGSLanguage.CodeType.Error_Invalid) & " - " & strFontName, New Font("Arial", 16, FontStyle.Bold), 10, 10, 500, 500, DevExpress.XtraPrinting.TextAlignment.MiddleCenter)})
                Return Me
            End Try

            xrLblEGS.Font = fntFooter

            fctMasterReport = Nothing

            With Me
                Select Case G_ReportOptions.dblReportType
                    Case 1, 2, 3, 6  'Recipe Standard,Modern, costing
                        If Not dtToPrint.Columns.Contains("ChildLevel") Then dtToPrint.Columns.Add(New DataColumn("ChildLevel", System.Type.GetType("System.Int16")))
                        Dim rw As DataRow
                        For Each rw In dtToPrint.Rows
                            rw("ChildLevel") = ChildLevel(rw, dtToPrint)
                        Next
                End Select
                .DataMember = dtToPrint.TableName.ToString
                .DataSource = dtToPrint

                .PaperKind = Printing.PaperKind.Custom
                .PageWidth = CInt(dblPageWidth) : .PageHeight = CInt(dblPageHeight)
                'Margins
                '------------
                .Margins.Left = 0 'CInt(dblLeftMargin)
                .Margins.Top = 0 'CInt(dblTopMargin)
                .Margins.Bottom = 0 'CInt(dblBottomMargin)
                .Margins.Right = 0 'CInt(dblRightMargin)

                'Orientation
                '-----------
                If blLandscape Then
                    .Landscape = True
                    intAvailableWidth = CInt(dblPageHeight)
                Else
                    .Landscape = False
                    intAvailableWidth = CInt(dblPageWidth)
                End If
                intAvailableWidth2 = intAvailableWidth

                Select Case DisplayRecipeDetails 'VRP 06.08.2008 
                    Case 2, 4 'ADF
                        Select Case G_ReportOptions.dblReportType
                            Case 1, 2, 3, 4, 6, 7, 21, 22, 23 'Recipe/Menu
                                With Me 'paper , orientation, width
                                    .PaperKind = New System.Drawing.Printing.PaperKind
                                    .PaperKind = Printing.PaperKind.A4
                                    .Margins = New System.Drawing.Printing.Margins(50.0!, 50.0!, 50.0!, 0.0!)
                                    .Landscape = False
                                    'intAvailableWidth = (.PageWidth - .Margins.Left - .Margins.Right)
                                    'intAvailableHeight = (.PageHeight - .Margins.Top - .Margins.Bottom)
                                End With
                        End Select
                End Select

                intAvailableWidth = intAvailableWidth - CInt(dblLeftMargin) - CInt(dblRightMargin)
                Select Case G_ReportOptions.dblReportType
                    Case 1, 2, 3  'Recipe Standard,Modern
                        intAvailableWidth2 = intAvailableWidth2 - 15
                        intAvailableWidth = intAvailableWidth - 15
                    Case 4 'Layout
                    Case 5  'Merchandise Details
                        intAvailableWidth2 = intAvailableWidth2 - 30
                        intAvailableWidth = intAvailableWidth - 30
                    Case 6  'Recipe Costing
                    Case 7 'mcm 12.01.05 Menu Costing
                    Case 21, 22, 23
                    Case 24
                End Select

                .xrLinePF.Left = dblLeftMargin
                .xrLinePF.Width = intAvailableWidth

                .xrLblEGS.Left = dblLeftMargin
                .xrLblEGS.Width = Int(intAvailableWidth / 2)

                .xrPIPageNumber.Left = .xrLblEGS.Width + dblLeftMargin
                .xrPIPageNumber.Width = Int(intAvailableWidth / 2)
                .xrPIPageNumber.Format = cLang.GetString(clsEGSLanguage.CodeType.Page) & " {0}/{1}"

                If NoPrintLines Then
                    .xrLinePF.Visible = False
                End If

                '---- Report Footer Logo and Address ------ 'DLS August282007
                subReportFooter(intAvailableWidth2 - CInt(dblRightMargin), strFontName)
                .Detail.Controls.AddRange(New XRControl() {lblID})
                lblID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "Code")

                CodePrintList = intCodePrintList

                Select Case G_ReportOptions.dblReportType
                    Case 1, 2, 3, 6  'Recipe Standard,Modern, costing
                        lblMain.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeListeMain")
                        lblParent.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeListeParent")
                        lblprintdetailsID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "ID")
                        lblprintdetailsID.Visible = False
                        lblParent.Visible = True
                End Select
                m_dblLeftMargin = dblLeftMargin
                Select Case DisplayRecipeDetails 'VRP 06.08.2008 
                    Case 2 'ADF
                        Select Case G_ReportOptions.dblReportType
                            Case 1, 2, 3, 4, 6, 7, 21, 22, 23 'Recipe/Menu

                                .SubRptDetail.Location = New System.Drawing.Point(0, 0)
                                SubRptDetail.Visible = True
                            Case Else
                                .SubRptDetail.Location = New System.Drawing.Point(dblLeftMargin, 0)
                                SubRptDetail.Visible = True
                        End Select
                    Case 4 'Recipe Center
                        .Margins.Bottom = 50
                        .SubRptDetail.Location = New System.Drawing.Point(0, 0)
                        SubRptDetail.Visible = True
                    Case Else
                        .SubRptDetail.Location = New System.Drawing.Point(dblLeftMargin, 0)
                        SubRptDetail.Visible = True
                End Select
            End With

            Return Me
        Catch ex As Exception
            Log.Info(ex.Message)
        End Try
    End Function

    Private Sub subReportFooter(ByVal intAvailableWidth As Integer, ByVal strFontName As String)
        Dim intX As Integer
        Dim intY As Integer

        With Me
            '---- Report Footer Logo and Address ------ 'DLS August282007
            If G_ReportOptions.strFooterLogoPath <> "" Then
                fntFooter = New Font(strFontName, 7, FontStyle.Regular)
                xrLblEGS.Font = fntFooter

                .XrFooterPicLogo.Image = Image.FromFile(G_ReportOptions.strFooterLogoPath)
                .XrFooterPicLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.AutoSize
                '.XrFooterPicLogo.Size = New System.Drawing.Size(156, 48)
                .XrFooterPicLogo.Location = New System.Drawing.Point(intAvailableWidth - XrFooterPicLogo.Width, 8)
                .XrFooterPicLogo.Visible = True

                .xrLblEGS.Font = fntFooter
                .xrLblEGS.Text = "" 'G_ReportOptions.strFooterAddress
                .xrLblEGS.Width = (intAvailableWidth - 10)
                .xrLblEGS.Height = 100
                .xrLblEGS.Multiline = True
                intX = .xrLblEGS.Location.X
                intY = .xrLblEGS.Location.Y
                SplitFooterAddress(G_ReportOptions.strFooterAddress, (intAvailableWidth - 10), intX, intY, DevExpress.XtraPrinting.TextAlignment.TopLeft)
                .PageFooter.Height = 100

                If intY > 100 Then
                    .PageFooter.Height = intY + 50
                End If
                XrFooterPicLogo.Top = xrLblEGS.Height - XrFooterPicLogo.Height + 20
                .xrPIPageNumber.Visible = False
            Else
                .XrFooterPicLogo.Visible = False
                .xrLblEGS.Location = New System.Drawing.Point(0, 8)
                .xrLblEGS.Text = ""
                .PageFooter.Height = 30
            End If
        End With
    End Sub

    Private Sub SplitFooterAddress(ByVal strNote As String, ByVal intAW As Integer, ByVal intCX As Integer, ByRef intCY As Integer, ByVal align As TextAlignment)
        With Me
            Dim intLineSpace As Integer = ReportingTextUtils.MeasureText("A", fntFooter, intAW, sf, Me.Padding).Height
            intLineSpace = CInt(GetLineSpace(intLineSpace * 3 / 5))
            intLineSpace -= 2

            strNote = strNote.Replace(Chr(10), "")
            Dim arr() As String = strNote.Split(vbCrLf)

            Dim i As Integer = 0
            While i < arr.Length
                If arr(i).Trim.Length > 0 Then
                    strNote = arr(i)
                    sf = New StringFormat(StringFormatFlags.NoClip)
                    intTextHeight = ReportingTextUtils.MeasureText("A", fntFooter, intAW, sf, Me.Padding).Height
                    'intTextHeight = fctGetHighest(strNote, intTextHeight, intAW + 10, fntFooter, sf, me.padding)
                    .PageFooter.Controls.AddRange(New XRControl() {fctMakeXRLabel(strNote, fntFooter, intCX, intCY, intAW, intTextHeight, align, True, , True)})
                    intCY += GetLineSpace(intTextHeight)
                Else
                    ' this was done to create another space
                    intCY += intLineSpace
                End If
                i += 1
            End While
        End With
    End Sub

    Private Function ChildLevel(ByVal rw As DataRow, ByVal dt As DataTable) As Integer
        If IsDBNull(rw("CodeListeParent")) OrElse rw("CodeListeParent") = 0 Then
            Return 0
        ElseIf rw("CodeListeParent") = rw("codeListeMain") Then
            Return 1
        Else
            Dim intLevel As Integer = 1
            Dim rowX As DataRow
            Dim intX As Integer = 0
            While Not IsDBNull(rw("CodeListeParent")) AndAlso rw("CodeListeParent") <> 0
                'rw = dt.Select("Code=" & rw("codeListeParent") & " AND CodeListeMain=" & rw("CodeListeMain"))(0)
                intX = 0
                For Each rowX In dt.Rows
                    If rowX("CodeListeMain") = rw("CodeListeMain") Then
                        If rowX("Code").ToString() = rw("codeListeParent").ToString() Then
                            rw = rowX
                            intX = 1
                        End If
                    End If
                Next
                If rowX Is Nothing Then Exit While
                intLevel += 1
            End While
            Return intLevel
        End If
    End Function

    'Private Sub SubRptDetail_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles SubRptDetail.BeforePrint
    '    Select Case G_ReportOptions.dblReportType 'VRP 06.08.2008 For Menu planning  TEST!!!
    '        Case 0 'No report Type
    '            Exit Sub
    '        Case 25
    '            SubRptDetail.ReportSource = New StandardDetail
    '            Dim DetailReport As StandardDetail = CType(SubRptDetail.ReportSource, StandardDetail)
    '            Dim strCodeTrans As String = Me.lblID.Text
    '            Dim strCode As String = Me.lblID2.Text
    '            Dim strCodeDay As String = Me.lblID3.Text
    '            Dim foundRows() As DataRow

    '            With G_ReportOptions
    '                Dim cLanguage As New clsLanguage(enumAppType.WebApp, strCnn)
    '                .intPageLanguage = cLanguage.GetCodeDictionary(IIf(strCodeTrans = "", 3, strCodeTrans))
    '                .intCodeTrans = IIf(strCodeTrans = "", 3, strCodeTrans)
    '                Dim cLang As New clsEGSLanguage(.intPageLanguage)

    '                Me.PageFooter.Visible = False

    '                Select Case MPPrintStyle
    '                    'Case enumMPStyle.A4HWLogo, enumMPStyle.A4HWOLogo
    '                    '    foundRows = .dtMPConfig.Select("CodeTrans=" & strCodeTrans)
    '                    '    DetailReport.fctPrintMenuPlanDetailSV_A4Hoch(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
    '                    'Case enumMPStyle.A4CWLogo, enumMPStyle.A4CWOLogo
    '                    '    foundRows = .dtMPConfig.Select("CodeTrans=" & strCodeTrans)
    '                    '    DetailReport.fctPrintMenuPlanDetailSV_A4Quer(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
    '                    Case enumMPStyle.AngebotshinweisA4H, enumMPStyle.AngebotshinweisA4H_auf, enumMPStyle.KennzeichnungA4C, enumMPStyle.KennzeichnungA4C_auf
    '                        If .dtPlan.Rows.Count > 0 Then
    '                            foundRows = .dtPlan.Select("CodeTrans=" & strCodeTrans & " AND Code=" & strCode)
    '                            DetailReport.fctPrintMenuPlanDetailSV_A4ShinweisKennzei(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
    '                        End If
    '                    Case enumMPStyle.EinlageblatterA5H, enumMPStyle.EinlageblatterA5H_auf
    '                        foundRows = .dtDetail.Select("Code=" & IIf(strCode = "", 0, strCode & " AND CodeTrans=" & G_ReportOptions.intCodeTrans & " AND CodeDay=" & strCodeDay))
    '                        DetailReport.fctPrintMenuPlanDetailSV_A5Einlageblaitter(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
    '                    Case enumMPStyle.EinlageblatterA6H, enumMPStyle.EinlageblatterA6H_auf
    '                        foundRows = .dtDetail.Select("Code=" & IIf(strCode = "", 0, strCode & " AND CodeTrans=" & G_ReportOptions.intCodeTrans & " AND CodeDay=" & strCodeDay))
    '                        DetailReport.fctPrintMenuPlanDetailSV_A6Einlageblaitter(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
    '                End Select

    '            End With
    '        Case Else
    '            SubRptDetail.ReportSource = New StandardDetail
    '            Dim DetailReport As StandardDetail = CType(SubRptDetail.ReportSource, StandardDetail)
    '            Dim strFilter As String
    '            Dim strFilter2 As String
    '            Dim foundRecipeDetails() As DataRow
    '            Dim foundKeywords() As DataRow
    '            Dim foundAllergens() As DataRow  'MCM 03.03.06
    '            Dim foundNotes() As DataRow  'MCM 03.03.06
    '            Dim strCode As String = Me.lblID.Text
    '            Dim strCodeParent As String = Me.lblParent.Text
    '            Dim strID As String = Me.lblprintdetailsID.Text
    '            Dim strCodeMain As String = Me.lblMain.Text
    '            Dim strStyle As String
    '            strStyle = "00" & VB.Right(G_ReportOptions.dblReportType.ToString, 1)
    '            Log.Info("StrCode: " & strCode & " | CodeUser: " & udtUser.Code & " | CodePrintList: " & CodePrintList & " | strCodeParent: " & strCodeParent & " | strCodeMain: " & strCodeMain)
    '            Dim strFilter3 As String
    '            With G_ReportOptions
    '                strFilter3 = "Code = " & strCode

    '                Select Case G_ReportOptions.dblReportType
    '                    Case 1, 2, 3, 6  'Recipe Standard,Modern, recipe costing
    '                        If strCodeParent.Length > 0 Then
    '                            strFilter3 = "Code = " & strCode & " AND CodeListeParent = " & strCodeParent & " AND CodeListeMain = " & strCodeMain & " AND ID = " & strID '& " AND CodeSet = " & G_ReportOptions.intSelectedNutrientSet 'ANM 1-15-2015 
    '                        ElseIf strCodeMain.Length > 0 Then
    '                            strFilter3 = "Code = " & strCode & " AND CodeListeParent IS NULL AND CodeListeMain = " & strCodeMain '& " AND CodeSet = " & G_ReportOptions.intSelectedNutrientSet 'ANM 1-15-2015 
    '                        Else
    '                            strFilter3 = "Code = " & strCode ' & " AND CodeSet = " & G_ReportOptions.intSelectedNutrientSet 'ANM 1-15-2015 
    '                        End If
    '                    Case Else
    '                End Select
    '                foundRecipeDetails = dtFoundDetail.Select(strFilter3) 'mcm 03.01.06
    '                Log.Info("strID: " & strID & " | strFilter3: " & strFilter3)
    '                strFilter = "CodeListe = " & strCode
    '                If foundRecipeDetails.Length <= 0 Then
    '                    Log.Info("no foundRecipeDetails")
    '                End If
    '                If Not IsNothing(.dtKeywords) Then
    '                    foundKeywords = .dtKeywords.Select(strFilter) 'mcm 03.01.06
    '                Else
    '                    foundKeywords = Nothing
    '                End If

    '                ''--- VRP 30.07.2008
    '                If DisplayRecipeDetails = 0 Then 'if Manor
    '                    strFilter = "CodeListe = " & strCode  'mcm 03.03.06
    '                    If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
    '                End If

    '                Me.xrPIPageNumber.Visible = False
    '                Select Case G_ReportOptions.dblReportType
    '                    Case 1, 2, 3  'Recipe Standard,Modern
    '                        If DisplayRecipeDetails = 4 Then 'Recipe Center
    '                            With Me
    '                                '.PaperKind = Printing.PaperKind.A4
    '                                .Margins = New System.Drawing.Printing.Margins(0, 50.0!, 0, 0)
    '                                intAvailableWidth = (.PageWidth - 50 - 50) - 25

    '                                intCurrentX = 0
    '                                intCurrentY = 0
    '                                .PageFooter.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, 0, intAvailableWidth, 1, 2)})
    '                                intCurrentY += 5

    '                                If File.Exists(G_strLogoPath) Then
    '                                    .PageFooter.Controls.AddRange(New XRControl() {fctMakePictureBox(G_strLogoPath, intCurrentX, intCurrentY, 80, 76)})
    '                                End If

    '                                intCurrentX += 100
    '                                strX = "Recipe printed from the " & SiteUrl & " website." & vbCrLf
    '                                strX += "A site developed by EGS SA - www.eg-software.com"
    '                                fntFont = New System.Drawing.Font("Calibri", 10, FontStyle.Regular)
    '                                .PageFooter.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntFont, Color.FromArgb(38, 38, 38), Color.Transparent, intCurrentX, intCurrentY, intAvailableWidth - 230, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, True)})


    '                                intCurrentX += intAvailableWidth - 220

    '                                If File.Exists(G_strLogoPath2) Then
    '                                    .PageFooter.Controls.AddRange(New XRControl() {fctMakePictureBox(G_strLogoPath2, intCurrentX, intCurrentY, 110, 150)})
    '                                End If
    '                            End With
    '                        End If

    '                        strFilter = "CodeListe =" & strCode
    '                        strFilter2 = "CodeListe =" & strCode & " AND ID =" & strID
    '                        'strFilter = "CodeListeMain =" & strCode
    '                        If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
    '                        If Not IsNothing(.dtDetail) Then foundRecipeDetails = .dtDetail.Select(strFilter2) Else foundRecipeDetails = Nothing
    '                        If Not IsNothing(.dtListeNote) Then foundNotes = .dtListeNote.Select(strFilter) Else foundNotes = Nothing
    '                        DetailReport.fctPrintRecipeDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont, DisplayRecipeDetails, strMigrosParam, strCnn, .dtSteps, , foundNotes, userLocale:=strUserLocale)
    '                    Case 4 'Layout
    '                        DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
    '                    Case 5  'Merchandise Details

    '                        strFilter = "CodeListe =" & strCode
    '                        If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing

    '                        DetailReport.fctPrintMerchandiseEgsDetails(foundRecipeDetails, foundKeywords, foundAllergens, .dtProductLink, strCnn, userLocale:=strUserLocale)
    '                    Case 6  'Recipe Costing
    '                        ' RDC 08.29.2013 : Added .dtListeNote
    '                        strFilter = "CodeListe =" & strCode
    '                        If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
    '                        DetailReport.fctPrintRecipeCostingEGSStandard4(foundRecipeDetails, foundKeywords, foundAllergens, dtStep, dtListeNote, strCnn, udtUser.Code, strCode, .dtNotes, strFilter3, G_ReportOptions.intTranslation, G_ReportOptions.intPageLanguage, userLocale:=strUserLocale)
    '                    Case 7 'mcm 12.01.05 Menu Costing
    '                        If DisplayRecipeDetails = 2 Then 'VRP 11.07.2008
    '                            DetailReport.fctPrintMenuDetailsADF(foundRecipeDetails, .dtSteps, strCnn, )
    '                        Else
    '                            DetailReport.fctPrintMenuCostingEGSStandard(foundRecipeDetails, foundKeywords, foundAllergens, strCnn, DisplayRecipeDetails, .dtSteps)
    '                        End If
    '                    Case 21, 22, 23 'mcm 13.01.06   Menu Details(Standard, Modern, Two Columns)
    '                        If DisplayRecipeDetails = 2 Then 'VRP 11.07.2008
    '                            DetailReport.fctPrintMenuDetailsADF(foundRecipeDetails, .dtSteps, strCnn, )
    '                        Else
    '                            strFilter = "CodeListe =" & strCode
    '                            If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
    '                            If Not IsNothing(.dtListeNote) Then foundNotes = .dtListeNote.Select(strFilter) Else foundNotes = Nothing 'KMQDC to add notes 2016.11.16
    '                            DetailReport.fctPrintMenuDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont, strCnn, DisplayRecipeDetails, .dtSteps, foundNotes)
    '                        End If
    '                    Case 24
    '                        DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
    '                    Case Else
    '                        Me.xrPIPageNumber.Visible = True
    '                End Select
    '            End With
    '    End Select

    '    ''SubRptDetail.ReportSource = New StandardDetail
    '    ''Dim DetailReport As StandardDetail = CType(SubRptDetail.ReportSource, StandardDetail)
    '    ''Dim strFilter As String
    '    ''Dim foundRecipeDetails() As DataRow
    '    ''Dim foundKeywords() As DataRow
    '    ''Dim foundAllergens() As DataRow  'MCM 03.03.06
    '    ''Dim strCode As String = Me.lblID.Text
    '    ''Dim strCodeParent As String = Me.lblParent.Text
    '    ''Dim strCodeMain As String = Me.lblMain.Text
    '    ''Dim strStyle As String
    '    ''strStyle = "00" & VB.Right(G_ReportOptions.dblReportType.ToString, 1)

    '    ''With G_ReportOptions
    '    ''    strFilter = "Code = " & strCode

    '    ''    Select Case G_ReportOptions.dblReportType
    '    ''        Case 1, 2, 3, 6  'Recipe Standard,Modern, recipe costing
    '    ''            If strCodeParent.Length > 0 Then
    '    ''                strFilter = "Code = " & strCode & " AND CodeListeParent = " & strCodeParent & " AND CodeListeMain = " & strCodeMain
    '    ''            ElseIf strCodeMain.Length > 0 Then
    '    ''                strFilter = "Code = " & strCode & " AND CodeListeParent IS NULL AND CodeListeMain = " & strCodeMain
    '    ''            Else
    '    ''                strFilter = "Code = " & strCode
    '    ''            End If
    '    ''        Case Else
    '    ''    End Select
    '    ''    foundRecipeDetails = .dtDetail.Select(strFilter) 'mcm 03.01.06

    '    ''    strFilter = "CodeListe =" & strCode
    '    ''    If Not IsNothing(.dtKeywords) Then
    '    ''        foundKeywords = .dtKeywords.Select(strFilter) 'mcm 03.01.06
    '    ''    Else
    '    ''        foundKeywords = Nothing
    '    ''    End If

    '    ''    ''--- VRP 30.07.2008
    '    ''    If DisplayRecipeDetails = 0 Then 'if Manor
    '    ''        strFilter = "CodeListe =" & strCode  'mcm 03.03.06
    '    ''        If Not IsNothing(.dtAllergens) Then
    '    ''            foundAllergens = .dtAllergens.Select(strFilter)
    '    ''        Else
    '    ''            foundAllergens = Nothing
    '    ''        End If
    '    ''    End If

    '    ''    Me.xrPIPageNumber.Visible = False
    '    ''    Select Case G_ReportOptions.dblReportType
    '    ''        Case 1, 2, 3  'Recipe Standard,Modern
    '    ''            DetailReport.fctPrintRecipeDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont, DisplayRecipeDetails, strMigrosParam, strCnn, .dtSteps)
    '    ''        Case 4 'Layout
    '    ''            If DisplayRecipeDetails = 2 Then
    '    ''                DetailReport.fctPrintRecipeDetailsADF(foundRecipeDetails, .dtSteps, strCnn)
    '    ''            Else
    '    ''                DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
    '    ''            End If
    '    ''        Case 5  'Merchandise Details
    '    ''            DetailReport.fctPrintMerchandiseEgsDetails(foundRecipeDetails, foundKeywords, foundAllergens, .dtProductLink)
    '    ''        Case 6  'Recipe Costing
    '    ''            If DisplayRecipeDetails = 2 Then
    '    ''                DetailReport.fctPrintRecipeDetailsADF(foundRecipeDetails, .dtSteps, strCnn)
    '    ''            Else
    '    ''                DetailReport.fctPrintRecipeCostingEGSStandard(foundRecipeDetails, foundKeywords, foundAllergens)
    '    ''            End If
    '    ''        Case 7 'mcm 12.01.05 Menu Costing
    '    ''            If DisplayRecipeDetails = 2 Then 'VRP 11.07.2008
    '    ''                DetailReport.fctPrintMenuDetailsADF(foundRecipeDetails, .dtSteps, strCnn)
    '    ''            Else
    '    ''                DetailReport.fctPrintMenuCostingEGSStandard(foundRecipeDetails, foundKeywords, foundAllergens, strCnn, DisplayRecipeDetails, .dtSteps)
    '    ''            End If
    '    ''        Case 21, 22, 23
    '    ''            'mcm 13.01.06   Menu Details(Standard, Modern, Two Columns)
    '    ''            If DisplayRecipeDetails = 2 Then 'VRP 11.07.2008
    '    ''                DetailReport.fctPrintMenuDetailsADF(foundRecipeDetails, .dtSteps, strCnn)
    '    ''            Else
    '    ''                DetailReport.fctPrintMenuDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont, strCnn, DisplayRecipeDetails, .dtSteps)
    '    ''            End If
    '    ''        Case 24
    '    ''            DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
    '    ''        Case Else
    '    ''            Me.xrPIPageNumber.Visible = True
    '    ''    End Select
    '    ''End With
    'End Sub
    Private Sub SubRptDetail_BeforePrint(ByVal sender As Object, ByVal e As CancelEventArgs) Handles SubRptDetail.BeforePrint
        Select Case G_ReportOptions.dblReportType 'VRP 06.08.2008 For Menu planning  TEST!!!
            Case 0 'No report Type
                Exit Sub
            Case 25
                SubRptDetail.ReportSource = New StandardDetail
                Dim DetailReport As StandardDetail = CType(SubRptDetail.ReportSource, StandardDetail)
                Dim strCodeTrans As String = Me.lblID.Text
                Dim strCode As String = Me.lblID2.Text
                Dim strCodeDay As String = Me.lblID3.Text
                Dim foundRows() As DataRow

                With G_ReportOptions
                    Dim cLanguage As New clsLanguage(enumAppType.WebApp, strCnn)
                    .intPageLanguage = cLanguage.GetCodeDictionary(IIf(strCodeTrans = "", 3, strCodeTrans))
                    .intCodeTrans = IIf(strCodeTrans = "", 3, strCodeTrans)
                    Dim cLang As New clsEGSLanguage(.intPageLanguage)

                    Me.PageFooter.Visible = False

                    Select Case MPPrintStyle
                        Case enumMPStyle.AngebotshinweisA4H, enumMPStyle.AngebotshinweisA4H_auf, enumMPStyle.KennzeichnungA4C, enumMPStyle.KennzeichnungA4C_auf
                            If .dtPlan.Rows.Count > 0 Then
                                foundRows = .dtPlan.Select("CodeTrans=" & strCodeTrans & " AND Code=" & strCode)
                                DetailReport.fctPrintMenuPlanDetailSV_A4ShinweisKennzei(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
                            End If
                        Case enumMPStyle.EinlageblatterA5H, enumMPStyle.EinlageblatterA5H_auf
                            foundRows = .dtDetail.Select("Code=" & IIf(strCode = "", 0, strCode & " AND CodeTrans=" & G_ReportOptions.intCodeTrans & " AND CodeDay=" & strCodeDay))
                            DetailReport.fctPrintMenuPlanDetailSV_A5Einlageblaitter(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
                        Case enumMPStyle.EinlageblatterA6H, enumMPStyle.EinlageblatterA6H_auf
                            foundRows = .dtDetail.Select("Code=" & IIf(strCode = "", 0, strCode & " AND CodeTrans=" & G_ReportOptions.intCodeTrans & " AND CodeDay=" & strCodeDay))
                            DetailReport.fctPrintMenuPlanDetailSV_A6Einlageblaitter(foundRows, udtUser, strCnn, SelectedWeek, MPPrintStyle, CodeUserPlan)
                    End Select

                End With
            Case Else
                SubRptDetail.ReportSource = New StandardDetail
                Dim DetailReport As StandardDetail = CType(SubRptDetail.ReportSource, StandardDetail)
                Dim strFilter As String
                Dim strFilter2 As String
                Dim foundRecipeDetails() As DataRow
                Dim foundKeywords() As DataRow
                Dim foundAllergens() As DataRow  'MCM 03.03.06
                Dim foundNotes() As DataRow  'MCM 03.03.06
                Dim strCode As String = Me.lblID.Text
                Dim strCodeParent As String = Me.lblParent.Text
                Dim strID As String = Me.lblprintdetailsID.Text
                Dim strCodeMain As String = Me.lblMain.Text
                Dim strStyle As String
                strStyle = "00" & VB.Right(G_ReportOptions.dblReportType.ToString, 1)
                Log.Info("StrCode: " & strCode & " | CodeUser: " & udtUser.Code & " | CodePrintList: " & CodePrintList & " | strCodeParent: " & strCodeParent & " | strCodeMain: " & strCodeMain)
                Dim strFilter3 As String
                With G_ReportOptions
                    strFilter3 = "Code = " & strCode

                    Select Case G_ReportOptions.dblReportType
                        Case 1, 2, 3, 6  'Recipe Standard,Modern, recipe costing
                            If strCodeParent.Length > 0 Then
                                strFilter3 = "Code = " & strCode & " AND CodeListeParent = " & strCodeParent & " AND CodeListeMain = " & strCodeMain & " AND ID = " & strID
                            ElseIf strCodeMain.Length > 0 Then
                                strFilter3 = "Code = " & strCode & " AND CodeListeParent IS NULL AND CodeListeMain = " & strCodeMain
                            Else
                                strFilter3 = "Code = " & strCode
                            End If
                        Case Else
                    End Select
                    foundRecipeDetails = dtFoundDetail.Select(strFilter3)
                    Log.Info("strID: " & strID & " | strFilter3: " & strFilter3)
                    strFilter = "CodeListe = " & strCode
                    If foundRecipeDetails.Length <= 0 Then
                        Log.Info("no foundRecipeDetails")
                    End If
                    If Not IsNothing(.dtKeywords) Then
                        foundKeywords = .dtKeywords.Select(strFilter)
                    Else
                        foundKeywords = Nothing
                    End If

                    If DisplayRecipeDetails = 0 Then 'if Manor
                        strFilter = "CodeListe = " & strCode
                        If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
                    End If

                    Me.xrPIPageNumber.Visible = False
                    Select Case G_ReportOptions.dblReportType
                        Case 1, 2, 3  'Recipe Standard,Modern
                            If DisplayRecipeDetails = 4 Then 'Recipe Center
                                With Me
                                    .Margins = New System.Drawing.Printing.Margins(0, 50.0!, 0, 0)
                                    intAvailableWidth = (.PageWidth - 50 - 50) - 25

                                    intCurrentX = 0
                                    intCurrentY = 0
                                    .PageFooter.Controls.AddRange(New XRControl() {fctMakeLine(intCurrentX, 0, intAvailableWidth, 1, 2)})
                                    intCurrentY += 5

                                    If File.Exists(G_strLogoPath) Then
                                        .PageFooter.Controls.AddRange(New XRControl() {fctMakePictureBox(G_strLogoPath, intCurrentX, intCurrentY, 80, 76)})
                                    End If

                                    intCurrentX += 100
                                    strX = "Recipe printed from the " & SiteUrl & " website." & vbCrLf
                                    strX += "A site developed by EGS SA - www.eg-software.com"
                                    fntFont = New System.Drawing.Font("Calibri", 10, FontStyle.Regular)
                                    .PageFooter.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntFont, Color.FromArgb(38, 38, 38), Color.Transparent, intCurrentX, intCurrentY, intAvailableWidth - 230, intTextHeight, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, True)})

                                    intCurrentX += intAvailableWidth - 220

                                    If File.Exists(G_strLogoPath2) Then
                                        .PageFooter.Controls.AddRange(New XRControl() {fctMakePictureBox(G_strLogoPath2, intCurrentX, intCurrentY, 110, 150)})
                                    End If
                                End With
                            End If

                            strFilter = "CodeListe =" & strCode
                            strFilter2 = "CodeListe =" & strCode & " AND ID =" & strID
                            If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
                            If Not IsNothing(.dtDetail) Then foundRecipeDetails = .dtDetail.Select(strFilter2) Else foundRecipeDetails = Nothing
                            If Not IsNothing(.dtListeNote) Then foundNotes = .dtListeNote.Select(strFilter) Else foundNotes = Nothing
                            DetailReport.fctPrintRecipeDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont, DisplayRecipeDetails, strMigrosParam, strCnn, .dtSteps, , foundNotes, userLocale:=strUserLocale)
                        Case 4 'Layout
                            DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
                        Case 5  'Merchandise Details
                            strFilter = "CodeListe =" & strCode
                            If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
                            DetailReport.fctPrintMerchandiseEgsDetails(foundRecipeDetails, foundKeywords, foundAllergens, .dtProductLink, strCnn, userLocale:=strUserLocale)
                        Case 6  'Recipe Costing
                            strFilter = "CodeListe =" & strCode
                            If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
                            DetailReport.fctPrintRecipeCostingEGSStandard4(foundRecipeDetails, foundKeywords, foundAllergens, dtStep, dtListeNote, strCnn, udtUser.Code, strCode, .dtNotes, strFilter3, G_ReportOptions.intTranslation, G_ReportOptions.intPageLanguage, userLocale:=strUserLocale)
                        Case 7 'Menu Costing
                            If DisplayRecipeDetails = 2 Then
                                DetailReport.fctPrintMenuDetailsADF(foundRecipeDetails, .dtSteps, strCnn, )
                            Else
                                DetailReport.fctPrintMenuCostingEGSStandard(foundRecipeDetails, foundKeywords, foundAllergens, strCnn, DisplayRecipeDetails, .dtSteps)
                            End If
                        Case 21, 22, 23 'Menu Details (Standard, Modern, Two Columns)
                            If DisplayRecipeDetails = 2 Then
                                DetailReport.fctPrintMenuDetailsADF(foundRecipeDetails, .dtSteps, strCnn, )
                            Else
                                strFilter = "CodeListe =" & strCode
                                If Not IsNothing(.dtAllergens) Then foundAllergens = .dtAllergens.Select(strFilter) Else foundAllergens = Nothing
                                If Not IsNothing(.dtListeNote) Then foundNotes = .dtListeNote.Select(strFilter) Else foundNotes = Nothing
                                DetailReport.fctPrintMenuDetailEgs(foundRecipeDetails, foundKeywords, foundAllergens, strStyle, DisplaySubRecipeAstrisk, DisplaySubRecipeNormalFont, strCnn, DisplayRecipeDetails, .dtSteps, foundNotes)
                            End If
                        Case 24
                            DetailReport.fctPrintRecipeEgsLayout(foundRecipeDetails)
                        Case Else
                            Me.xrPIPageNumber.Visible = True
                    End Select
                End With
        End Select
    End Sub

#Region " MENU PLAN "
    Function fctMasterReportPlan(ByVal dtToPrint As DataTable, ByVal strConnection As String, _
                                     ByVal strSelectedWeek As String) As XtraReport 'VRP 06.08.2008

        With Me
            .DataMember = dtToPrint.TableName.ToString
            .DataSource = dtToPrint

            .Detail.Controls.AddRange(New XRControl() {lblID, lblID2})
        End With

        Try
            Select Case MPPrintStyle
                Case enumMPStyle.A4HWLogo, enumMPStyle.A4HWOLogo, enumMPStyle.A4CWLogo, enumMPStyle.A4CWOLogo
                    lblID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeTrans")
                    lblID2.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "Code")
                    lblID.Visible = True '
                    lblID2.Visible = False

                    fntReportTitle = New Font("Arial Black", 13, FontStyle.Bold)
                    fntBold = New Font("Arial Black", 9, FontStyle.Bold)
                    fntRegular = New Font("Arial", 9, FontStyle.Regular)
                    fntFont = New Font("Arial", 7.0!, FontStyle.Regular)
                Case enumMPStyle.AngebotshinweisA4H, enumMPStyle.AngebotshinweisA4H_auf
                    lblID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeTrans")
                    lblID2.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "Code")
                    lblID.Visible = True '
                    lblID2.Visible = False

                    fntReportTitle = New Font("Arial Black", 40, FontStyle.Bold)
                    fntBold = New Font("Arial Black", 25, FontStyle.Bold)
                    fntRegular = New Font("Arial", 25, FontStyle.Regular)
                Case enumMPStyle.KennzeichnungA4C, enumMPStyle.KennzeichnungA4C_auf
                    lblID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeTrans")
                    lblID2.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "Code")
                    lblID.Visible = True '
                    lblID2.Visible = False

                    fntReportTitle = New Font("Arial Black", 45, FontStyle.Bold)
                    fntBold = New Font("Arial Black", 25, FontStyle.Bold)
                    fntRegular = New Font("Arial", 25, FontStyle.Regular)
                Case enumMPStyle.EinlageblatterA5H, enumMPStyle.EinlageblatterA5H_auf
                    Me.Detail.Controls.AddRange(New XRControl() {lblID, lblID2, lblID3})

                    lblID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeTrans")
                    lblID.Visible = True
                    lblID2.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "Code")
                    lblID2.Visible = False
                    lblID3.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeDay")
                    lblID3.Visible = False

                    fntReportTitle = New Font("Arial Black", 30, FontStyle.Bold)
                    fntBold = New Font("Arial Black", 25, FontStyle.Bold)
                    fntRegular = New Font("Arial", 25, FontStyle.Regular)
                Case enumMPStyle.EinlageblatterA6H, enumMPStyle.EinlageblatterA6H_auf
                    Me.Detail.Controls.AddRange(New XRControl() {lblID, lblID2, lblID3})

                    lblID.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeTrans")
                    lblID.Visible = True
                    lblID2.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "Code")
                    lblID2.Visible = False
                    lblID3.DataBindings.Add("Text", dtToPrint, dtToPrint.TableName.ToString + "." + "CodeDay")
                    lblID3.Visible = False

                    fntReportTitle = New Font("Arial Black", 19, FontStyle.Bold)
                    fntBold = New Font("Arial Black", 12, FontStyle.Bold)
                    fntRegular = New Font("Arial", 12, FontStyle.Regular)
            End Select
        Catch ex As Exception
            Return Me
        End Try

        With Me 'paper , orientation, width
            .PaperKind = New System.Drawing.Printing.PaperKind
            .PaperKind = Printing.PaperKind.A4

            Select Case MPPrintStyle
                Case enumMPStyle.A4HWLogo, enumMPStyle.A4HWOLogo, enumMPStyle.A4CWLogo, enumMPStyle.A4CWOLogo
                    .Margins = New System.Drawing.Printing.Margins(50.0!, 0, 50.0!, 0)
                Case enumMPStyle.AngebotshinweisA4H
                    .Margins = New System.Drawing.Printing.Margins(150.0!, 0, 200.0!, 0)
                Case enumMPStyle.AngebotshinweisA4H_auf
                    .Margins = New System.Drawing.Printing.Margins(250.0!, 0, 200.0!, 0)
                Case enumMPStyle.KennzeichnungA4C
                    .Margins = New System.Drawing.Printing.Margins(100.0!, 0, 50.0!, 0)
                Case enumMPStyle.KennzeichnungA4C_auf
                    .Margins = New System.Drawing.Printing.Margins(257.0!, 0, 150.0!, 0)
                Case enumMPStyle.EinlageblatterA5H
                    .Margins = New System.Drawing.Printing.Margins(100.0!, 0, 140.0!, 0)
                Case enumMPStyle.EinlageblatterA5H_auf
                    .Margins = New System.Drawing.Printing.Margins(150.0!, 0, 150.0!, 0)
                Case enumMPStyle.EinlageblatterA6H
                    .Margins = New System.Drawing.Printing.Margins(100.0!, 0, 50.0!, 0)
                Case enumMPStyle.EinlageblatterA6H_auf
                    .Margins = New System.Drawing.Printing.Margins(125.0!, 0, 118.0!, 0.0!)
            End Select

            .Landscape = G_ReportOptions.blLandscape
            intAvailableWidth = (.PageWidth - .Margins.Left - .Margins.Right)
            intAvailableHeight = (.PageHeight - .Margins.Top - .Margins.Bottom)
        End With

        With Me
            .SubRptDetail.Location = New System.Drawing.Point(0, 0)
            SubRptDetail.Visible = True
        End With
        Return Me
    End Function

    Private Function fctGetDay(ByVal intCodeDay As Integer, ByVal cLang As clsEGSLanguage) As String 'intCodetrans
        Select Case intCodeDay
            Case 1 : Return cLang.GetString(clsEGSLanguage.CodeType.Monday)
            Case 2 : Return cLang.GetString(clsEGSLanguage.CodeType.Tuesday)
            Case 3 : Return cLang.GetString(clsEGSLanguage.CodeType.Wednesday)
            Case 4 : Return cLang.GetString(clsEGSLanguage.CodeType.Thursday)
            Case 5 : Return cLang.GetString(clsEGSLanguage.CodeType.Friday)
            Case 6 : Return cLang.GetString(clsEGSLanguage.CodeType.Saturday)
            Case 7 : Return cLang.GetString(clsEGSLanguage.CodeType.Sunday)
        End Select
    End Function

    Private Function fctGetPrice(ByVal intCodePrice As Integer) As String 'TEMPORARY
        Select Case intCodePrice
            Case 1 : Return "INT"
            Case 2 : Return "EXT"
        End Select
    End Function

    Private Sub SplitPanelText(ByVal xrPanel As XRPanel, ByVal strX As String, ByVal intCX As Integer, ByRef intCY As Integer, ByVal align As TextAlignment)
        With Me
            Dim intLineSpace As Integer = ReportingTextUtils.MeasureText("A", fntRegular, xrPanel.Width, sf, Me.Padding).Height
            intLineSpace = CInt(GetLineSpace(intLineSpace * 3 / 5))

            strX = strX.Replace(Chr(10), "")
            Dim arr() As String = strX.Split(vbCrLf)

            Dim i As Integer = 0
            While i < arr.Length
                If arr(i).Trim.Length > 0 Then
                    strX = arr(i)
                    sf = New StringFormat(StringFormatFlags.NoClip)
                    intTextHeight = ReportingTextUtils.MeasureText(strX, fntRegular, xrPanel.Width, sf, Me.Padding).Height

                    xrPanel.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntRegular, Color.Black, Color.Transparent, intCX, intCY, xrPanel.Width, intTextHeight, align, True, True, True)})
                    intCY += intTextHeight
                Else
                    intCY += intLineSpace
                End If
                i += 1
            End While
        End With
    End Sub

    Function fctMakeXrPanel(ByVal intX As Integer, ByVal intY As Integer, ByVal intWidth As Integer, ByVal strName As String, _
                            ByVal BackColor As Color, Optional ByVal intHeight As Integer = 1) 'VRP 05.06.2008
        Dim xrPanel As New XRPanel
        With xrPanel
            .KeepTogether = True
            .Location = New System.Drawing.Point(intX, intY)
            .Width = intWidth
            .Height = intHeight
            .Name = strName
            .CanGrow = True
            .BackColor = BackColor
        End With
        Return xrPanel
    End Function
#End Region
    Private Sub xrReports_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
      
    End Sub
End Class
Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsSite
'Decription         : Manages Site Table
'Date Created       : 26.9.2005
'Author             : JRL
'Revision History   : 
'                       Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'                       jhl 29.12.05 Added Function to get Code Site By SiteName
#End Region
Public Class clsSite
    Inherits clsDBRoutine
    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeUser As Int32 = -1
    Private L_lngCodeSite As Int32 = -1


    'Properties
    Private L_AppType As enumAppType
    Private L_User As structUser
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


#Region "Get Methods"


    Public Function GetKIOSKSite() As Object
        Return FetchList(-1, -1, "", 2)
    End Function

    ''' <summary>
    ''' Return Site
    ''' </summary>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeSite As Integer) As Object
        Return FetchList(intCodeSite, -1)
    End Function

    ''' <summary>
    ''' Get List of all sites
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList() As Object
        Return FetchList(-1, -1)
    End Function
    ''' <summary>
    ''' Get List of all sites
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal strSiteName As String) As Object
        Return FetchList(-1, -1, strSiteName)
    End Function
    ''' <summary>
    ''' Fetch List of Sites belonging to a property
    ''' </summary>
    ''' <param name="intCodeProperty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListByProperty(ByVal intCodeProperty As Integer) As Object
        Return FetchList(-1, intCodeProperty)
    End Function

    ''' <summary>
    ''' Fetch List of Sites belonging to a property
    ''' </summary>
    ''' <param name="intCodeProperty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListByProperty(ByVal intCodeProperty As Integer, intParent As Integer, strTable As String) As Object
        Return FetchList(-1, intCodeProperty, intParent:=intParent, strTable:=strTable)
    End Function


    ''' <summary>
    ''' Fetch sites accessible to the current user
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListByCodeUser(ByVal intCodeUser As Integer) As Object
        Dim strCommandText As String = "sp_EgswSitesGetListByCodeUser"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
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
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    ''' <summary>
    ''' Get sites by marking.
    ''' </summary>
    ''' <param name="intCodeUser">The code of the user.</param>        
    ''' <param name="EMarkFilter">One of FilterMark values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListMark(ByVal intCodeUser As Int32, ByVal EMarkFilter As FilterMark) As Object
        'vbv 23.03.2006
        Return FetchListMark(intCodeUser, EMarkFilter)

    End Function

    Public Function GetCodeSite(ByVal strSiteName As String) As Int32
        Dim dr As SqlDataReader = CType(GetList(strSiteName), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return CInt((dr.GetValue(dr.GetOrdinal("Code")).ToString))
            Loop
        Else
            Return 0
        End If
        dr.Close()
    End Function
    Public Function GetSiteName(ByVal lngCode As Int32) As String
        Dim dr As SqlDataReader = CType(GetList(lngCode), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return (dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
        Else
            Return ""
        End If
        dr.Close()
    End Function

    Public Function GetSiteCodeName(ByVal intexCodeSite As Integer, Optional ByVal intCodeProperty As Integer = -1) As Object
        Dim strCommandText As String = "GET_SITESCODENAME"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@ExCludeCodeSite", intexCodeSite)
        arrParam(1) = New SqlParameter("@CodeProperty", intCodeProperty)

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


    Public Function GetRSSFeedsStats() As DataSet
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim ds As New DataSet
        With cmd
            .Connection = cn
            .CommandText = "sp_GetRSSFeedsStats"
            .CommandType = CommandType.StoredProcedure


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

#End Region
#Region "Save Methods"
    ''' <summary>
    ''' Save Site
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="strName"></param>
    ''' <param name="intGroup"></param>
    ''' <param name="SiteLevel"></param>
    ''' <param name="intCodeUser"></param>
    ''' <param name="TranMode"></param>
    ''' <param name="strTranslationCodeList"></param>
    ''' <param name="intCodeSiteCopyLocalItemsFrom">Copy all local items like category of a site. Value is zero if not used.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateSite(ByRef intCode As Integer, ByVal strName As String, ByVal intGroup As Integer, ByVal SiteLevel As enumGroupLevel, ByVal intCodeUser As Integer, ByVal TranMode As enumEgswTransactionMode, ByVal strTranslationCodeList As String, Optional ByVal intCodeSiteCopyLocalItemsFrom As Integer = 0) As enumEgswErrorCode
        Try
            Dim arrParam(8) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@tntTranMode", TranMode)
            arrParam(2) = New SqlParameter("@intCode", intCode)
            arrParam(2).Direction = ParameterDirection.InputOutput
            arrParam(3) = New SqlParameter("@nvcName", strName)
            arrParam(4) = New SqlParameter("@intGroup", intGroup)
            arrParam(5) = New SqlParameter("@intSiteLevel", SiteLevel)
            arrParam(6) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(7) = New SqlParameter("@vcTranslationCodeList", strTranslationCodeList)
            arrParam(8) = New SqlParameter("@intCopyLocalItemsOfThisCodeSite", intCodeSiteCopyLocalItemsFrom)            

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswSiteUpdate", arrParam)
            intCode = CInt(arrParam(2).Value)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Function UpdateSite(ByRef intCode As Integer, ByVal strName As String, ByVal intGroup As Integer, ByVal SiteLevel As enumGroupLevel, ByVal intCodeUser As Integer, ByVal TranMode As enumEgswTransactionMode, ByVal strTranslationCodeList As String, ByVal strRefName As String, Optional ByVal intCodeSiteCopyLocalItemsFrom As Integer = 0) As enumEgswErrorCode
        Try
            Dim arrParam(9) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@tntTranMode", TranMode)
            arrParam(2) = New SqlParameter("@intCode", intCode)
            arrParam(2).Direction = ParameterDirection.InputOutput
            arrParam(3) = New SqlParameter("@nvcName", strName)
            arrParam(4) = New SqlParameter("@intGroup", intGroup)
            arrParam(5) = New SqlParameter("@intSiteLevel", SiteLevel)
            arrParam(6) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(7) = New SqlParameter("@vcTranslationCodeList", strTranslationCodeList)
            arrParam(8) = New SqlParameter("@intCopyLocalItemsOfThisCodeSite", intCodeSiteCopyLocalItemsFrom)
            arrParam(9) = New SqlParameter("@strRefName", strRefName)

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswSiteUpdate", arrParam)
            intCode = CInt(arrParam(2).Value)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function
    ''' <summary>
    ''' Copy all local basic items to another site
    ''' </summary>
    ''' <param name="intCodeSiteSource"></param>
    ''' <param name="intCodeSiteNew"></param>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CopyLocalItems(ByVal intCodeSiteSource As Integer, ByVal intCodeSiteNew As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Try
            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@intCodeSiteSource", intCodeSiteSource)
            arrParam(2) = New SqlParameter("@intCodeSiteNew", intCodeSiteNew)
            arrParam(3) = New SqlParameter("@intCOdeUserOwner", intCodeUser)

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "SITE_CopyLocalItemsToAnotherSite", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function
#End Region
#Region "Remove Methods"
    ''' <summary>
    ''' Delete one item from the list
    ''' </summary>
    ''' <param name="intCode">Code of the site. </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFromList(ByVal intCode As Integer) As enumEgswErrorCode
        Return DeleteFromList(intCode, "", enumEgswTransactionMode.Delete)
    End Function

#End Region

#Region "Private Methods"
    Private Function FetchListMark(ByVal intCodeUser As Int32, _
       ByVal EFilterMark As FilterMark) As Object
        'vbv 23.03.2006
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader = Nothing
        Dim cmd As New SqlCommand
        Dim intCodeProperty As Int32 = -1

        If L_User.RoleLevelHighest = 0 Then 'Get ALL items
            intCodeProperty = -1
        ElseIf L_User.RoleLevelHighest = 1 Then 'Get ALL items for a site
            intCodeProperty = L_User.Site.Group
        ElseIf L_User.RoleLevelHighest = 2 Then 'Get ALL items for a property
            intCodeProperty = L_User.Site.Group
        End If
        intCodeProperty = -1 'temp to override value and get all sites

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SITE_GetListMark"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = intCodeProperty
                .Parameters.Add("@tntMode", SqlDbType.TinyInt).Value = EFilterMark
            End With

            If L_bytFetchType = enumEgswFetchType.DataReader Then
                cmd.Connection.Open()
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            ElseIf L_bytFetchType = enumEgswFetchType.DataTable Then
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

            ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
                With da
                    .SelectCommand = cmd
                    .Fill(ds, "SiteList")
                End With
            End If

        Catch ex As Exception
            dr = Nothing
            ds = Nothing
            dt.Dispose()
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        If L_bytFetchType = enumEgswFetchType.DataReader Then
            Return dr
        ElseIf L_bytFetchType = enumEgswFetchType.DataTable Then
            Return dt
        ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
            Return ds
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Fetch site list
    ''' </summary>
    ''' <param name="intCodeSite"></param>
    ''' <param name="intCodeProperty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FetchList(ByVal intCodeSite As Integer, ByVal intCodeProperty As Integer, Optional ByVal strSiteName As String = "", Optional ByVal intSiteLevel As Integer = -1, _
                               Optional intParent As Integer = -99, Optional strTable As String = "") As Object
        Dim strCommandText As String = "sp_EgswSiteGetList"

        Dim arrParam(5) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intCodeProperty", intCodeProperty)
        arrParam(2) = New SqlParameter("@vchName", strSiteName)
        If intSiteLevel = -1 Then
            arrParam(3) = New SqlParameter("@siteLevel", DBNull.Value)
        Else
            arrParam(3) = New SqlParameter("@siteLevel", intSiteLevel)
        End If

        arrParam(4) = New SqlParameter("@intParent", intParent)
        arrParam(5) = New SqlParameter("@strTable", strTable)

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

    ''' <summary>
    ''' Delete list of sites or one site
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="strCodeList"></param>
    ''' <param name="TrandMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DeleteFromList(ByVal intCode As Integer, ByVal strCodeList As String, ByVal TrandMode As enumEgswTransactionMode) As enumEgswErrorCode

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", "")
        arrParam(0).Direction = ParameterDirection.ReturnValue
        arrParam(1) = New SqlParameter("@intCode", intCode)
        arrParam(2) = New SqlParameter("@tntTranMode", TrandMode)
        arrParam(3) = New SqlParameter("@txtCodeList", strCodeList)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswSiteDelete", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Other Public Functions and Methods"
    Public Sub PopulateSiteList(ByVal cbo As Windows.Forms.ComboBox)
        Dim lngCodeProp As Int32

        If L_User.RoleLevelHighest = 0 Then
            lngCodeProp = -1
        Else
            lngCodeProp = L_User.Site.Group
        End If
        Dim dr As SqlDataReader = CType(GetListByProperty(lngCodeProp), SqlDataReader) 'clsSite.FetchSiteList(lngCodeProp)
        If dr IsNot Nothing Then
            Do While dr.Read
                Debug.Print(dr.GetValue(dr.GetOrdinal("Name")).ToString)
                cbo.Items.Add(dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
        End If

        dr.Close()
        dr = Nothing
    End Sub
#End Region

End Class

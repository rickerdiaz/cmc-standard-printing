Imports System.Data.SqlClient
Imports System.Data
Public Class clsTerminal

#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_User As structUser
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32
#End Region

#Region "Class Functions and Properties"
    Public Sub New(ByVal udtUser As structUser, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_User = udtUser
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try
    End Sub

    Public ReadOnly Property ItemsNotDeleted() As DataTable
        Get
            ItemsNotDeleted = L_dtList
        End Get
    End Property

#End Region

#Region "Private Methods"
    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
    Optional ByVal strName As String = "", Optional ByVal lngCodePOS As Int32 = -1) As Object

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1

        If L_User.RoleLevelHighest = 0 Then 'Get ALL items
            lngCodeProperty = -1
        ElseIf L_User.RoleLevelHighest = 1 Then 'Get ALL items for a site
            lngCodeSite = L_User.Site.Code
        ElseIf L_User.RoleLevelHighest = 2 Then 'Get ALL items for a property
            lngCodeProperty = L_User.Site.Group
        End If

        dr = Nothing
        FetchList = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswTerminalGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                If strName.Trim <> "" Then _
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 150).Value = strName
                .Parameters.Add("@intCodePOS", SqlDbType.Int).Value = lngCodePOS
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
                    .Fill(ds, "ItemList")
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
        End If

    End Function

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
    ByRef lngCode As Int32, ByVal udtTerminal As structTerminal, ByVal strCodeSiteList As String, _
    ByVal strCodeTerminalList As String, ByVal TranMode As enumEgswTransactionMode, _
    Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                If oTransaction Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = oTransaction.Connection
                    .Transaction = oTransaction
                End If

                .CommandText = "sp_EgswTerminalUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 35).Value = udtTerminal.Number
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 35).Value = udtTerminal.Name
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@intDateFormat", SqlDbType.Int).Value = udtTerminal.DateFormat
                .Parameters.Add("@nvcDateSeparator", SqlDbType.NVarChar, 1).Value = udtTerminal.DateSeparator
                .Parameters.Add("@nvcThouSeparator", SqlDbType.NVarChar, 1).Value = udtTerminal.ThouSeparator
                .Parameters.Add("@nvcDecimalSeparator", SqlDbType.NVarChar, 1).Value = udtTerminal.DecimalSeparator
                .Parameters.Add("@intTimeFormat", SqlDbType.Int).Value = udtTerminal.TimeFormat
                .Parameters.Add("@nvcTimeSeparator", SqlDbType.NVarChar, 1).Value = udtTerminal.TimeSeparator
                .Parameters.Add("@nvcListSeparator", SqlDbType.NVarChar, 10).Value = udtTerminal.ListSeparator
                .Parameters.Add("@intCodePOS", SqlDbType.Int).Value = udtTerminal.CodePOS
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 500).Value = udtTerminal.Note
                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                'strCodeSiteList.Trim()
                'If strCodeSiteList <> "" Then
                '    If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
                '        Return enumEgswErrorCode.InvalidCodeList
                '    Else
                '        .Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = strCodeSiteList
                '    End If
                'End If

                strCodeTerminalList.Trim()
                If strCodeTerminalList <> "" Then
                    If Not (strCodeTerminalList.StartsWith("(") And strCodeTerminalList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeTerminalList
                    End If
                End If

                '.Parameters("@retval").Direction = ParameterDirection.ReturnValue
                '.Parameters("@intCode").Direction = ParameterDirection.ReturnValue
                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                lngCode = CInt(.Parameters("@intCode").Value)
            End With

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If oTransaction Is Nothing AndAlso cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If oTransaction Is Nothing AndAlso cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Private Function RemoveFromList(ByVal lngCode As Int32, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal strCodeList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32

        If L_User.RoleLevelHighest = 0 Then 'Unshare to ALL 
            lngCodeProperty = -1
        Else 'Unshare to ALL sites belonging to a property or Unshare to self
            lngCodeProperty = L_User.Site.Group
        End If

        'IsGlobal = L_User.RoleLevelHighest = 0

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswTerminalDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                '.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = L_User.Code
                '.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                '.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                '.Parameters.Add("@tntType", SqlDbType.TinyInt).Value = dataListItemType
                strCodeList.Trim()
                If strCodeList <> "" Then
                    If Not (strCodeList.StartsWith("(") And strCodeList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@nvchCodeList", SqlDbType.NVarChar, 4000).Value = strCodeList
                    End If
                End If

                'If TranMode = enumEgswTransactionMode.ModifyStatus Then
                '    .Parameters.Add("@bytStatus", SqlDbType.TinyInt).Value = bytStatus
                'End If

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

        If L_ErrCode = enumEgswErrorCode.OneItemNotDeleted Then
            Dim da As New SqlDataAdapter

            Try
                cmd.CommandText = "sp_EgswItemGetNotDeleted"
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Clear()
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = L_User.Code
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswTerminal"

                L_dtList = New DataTable
                With da
                    .SelectCommand = cmd
                    L_dtList.BeginLoadData()
                    .Fill(L_dtList)
                    L_dtList.EndLoadData()
                End With
            Catch ex As Exception
                L_dtList.Dispose()
                If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
                cmd.Dispose()
                Throw New Exception(ex.Message, ex)
            End Try
        End If

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode
    End Function

    ''' <summary>
    ''' Updates the position of items.
    ''' </summary>
    ''' <param name="strCodeList">The list of item codes to be moved.</param>
    ''' <param name="flagMoveUp"></param>
    ''' <param name="lngCodeSite">The CodeSite of the items to be moved.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdatePosition(ByVal strCodeList As String, ByVal flagMoveUp As Boolean, _
        ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListItemType) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswTerminalMovePos"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@txtCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                .Parameters.Add("@bitMoveUp", SqlDbType.TinyInt).Value = flagMoveUp
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intListeType", SqlDbType.Int).Value = eListeType

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function
#End Region

#Region "Get Methods"
    '''' <summary>
    '''' Get all Terminals.
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks></remarks>    
    'Public Overloads Function GetList() As Object
    '    Return FetchList(-1, -1)
    'End Function

    ''' <summary>
    ''' Get a Terminal by Name.
    ''' </summary>
    ''' <param name="strName">The name of the Terminal to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String) As Object
        Return FetchList(-1, -1, strName)
    End Function

    ''' <summary>
    ''' Get a Terminal by Code or by Site
    ''' </summary>
    ''' <param name="lngCode">The Code of the Terminal to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32, ByVal lngCodeSite As Int32) As Object 'DataTable
        Return FetchList(lngCodeSite, lngCode)
    End Function


    ''' <summary>
    ''' Get Terminal by Code, by Site, or by POS Type
    ''' </summary>
    ''' <param name="lngCode">The Code of the Terminal to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32, ByVal lngCodeSite As Int32, ByVal lngCodePOS As Int32) As Object 'DataTable
        Return FetchList(lngCodeSite, lngCode, "", lngCodePOS)
    End Function

    Public Function GetTerminalName(ByVal lngCode As Int32) As String
        Dim dr As SqlDataReader = CType(GetList(lngCode, -1), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return (dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
        Else
            Return ""
        End If
    End Function
    Public Function GetCodeTerminal(ByVal strTerminalName As String) As Int32
        Dim dr As SqlDataReader = CType(GetList(strTerminalName), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return CInt((dr.GetValue(dr.GetOrdinal("Code")).ToString))
            Loop
        Else
            Return 0
        End If
    End Function
#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Updates Terminal
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Terminal to be updated.</param>
    ''' <param name="udtTerminal">One of the structTerminal values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtTerminal As structTerminal) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtTerminal, "", "", _
             CType(IIf(lngCode < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Merge Terminals
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeTerminalList">The list of Terminal Codes to be merged.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeTerminalList As String, ByVal udtTerminal As structTerminal) As enumEgswErrorCode
        Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtTerminal, "", strCodeTerminalList, enumEgswTransactionMode.MergeDelete)
    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Terminal List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Overloads Function Remove(ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode

    '    'Return RemoveFromList(-1, -1, -1, False, enumEgswTransactionMode.Purge)
    '    Return RemoveFromList(L_User.Code, -1, -1, False, enumEgswTransactionMode.Purge, dataListItemType)

    'End Function

    ''' <summary>
    ''' Deletes a Terminal.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Terminal to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCode As Int32) As enumEgswErrorCode
        Return RemoveFromList(lngCode, enumEgswTransactionMode.Delete)
    End Function

    ''' <summary>
    ''' Deletes Terminals specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="strCodeList">The list of Terminal Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function Remove(ByVal strCodeList As String) As enumEgswErrorCode
        Return RemoveFromList(-1, enumEgswTransactionMode.MultipleDelete, strCodeList)
    End Function

#End Region

#Region "Other Public Functions and Methods"
    Public Sub PopulateTerminalList(ByVal cbo As Windows.Forms.ComboBox, ByVal intCodeSite As Int32, ByVal intCodePOS As Int32)
        cbo.ResetText()
        cbo.Items.Clear()
        Dim dr As SqlDataReader = CType(GetList(-1, intCodeSite, intCodePOS), SqlDataReader)
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

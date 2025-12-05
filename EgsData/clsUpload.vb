Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsUpload
'Decription         : Manages UploadConfig Table
'Date Created       : 04.07.2007
'Author             : JHL
'Revision History   : 
#End Region

''' <summary>
''' Manages UploadConfig Table
''' </summary>
''' <remarks></remarks>

Public Class clsUpload


#Region "Variable Declarations / Dependencies"

    Private L_ErrCode As enumEgswErrorCode
    Private L_intCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_udtUser As structUser
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_intCode As Int32
    Private L_strName As String
    Private L_ItemType As enumDataListItemType
    Private L_IsGlobal As Boolean

#End Region

#Region "Class Functions and Properties"
#Region "Functions"
    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
       Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader, _
       Optional ByVal CreateRecord As Boolean = False)

        Try
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn

            If CreateRecord Then
            End If
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        ClearMarkings() 'items marked as not deleted
        MyBase.Finalize()
    End Sub

#End Region

#Region "Properties"
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

    Public Property Code() As Int32
        Get
            Code = L_intCode
        End Get
        Set(ByVal value As Int32)
            L_intCode = value
        End Set
    End Property

    Public Property Type() As enumDataListItemType
        Get
            Type = L_ItemType
        End Get
        Set(ByVal value As enumDataListItemType)
            L_ItemType = value
        End Set
    End Property

    Public Property IsGlobal() As Boolean
        Get
            IsGlobal = L_IsGlobal
        End Get
        Set(ByVal value As Boolean)
            L_IsGlobal = value
        End Set
    End Property
#End Region

#End Region

#Region "Private Methods"

    Private Function FetchList(ByVal lngCode As Int32) As Object


        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "UPLOADConfig_GetList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
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

        Return Nothing
    End Function

    Private Function SaveIntoList(ByRef lngCode As Int32, ByVal udtUpload As structUpload, ByVal TranMode As enumEgswTransactionMode, _
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

                .CommandText = "UPLOADConfig_Update"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtUpload.Code
                .Parameters.Add("@nvcHostName", SqlDbType.NVarChar, 50).Value = udtUpload.HostName
                .Parameters.Add("@nvcIPAddress ", SqlDbType.NVarChar, 20).Value = udtUpload.IPAddress
                .Parameters.Add("@intFileType", SqlDbType.Int).Value = udtUpload.FileType
                .Parameters.Add("@nvcPath", SqlDbType.NVarChar, 50).Value = udtUpload.Path
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode

                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue


                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
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

    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
         ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
         Optional ByVal bytStatus As Byte = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32

        If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
            lngCodeProperty = -1
        Else 'Unshare to ALL sites belonging to a property or Unshare to self
            lngCodeProperty = L_udtUser.Site.Group
        End If

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "UPLOADConfig_Delete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                strCodeList.Trim()
                If strCodeList <> "" Then
                    If Not (strCodeList.StartsWith("(") And strCodeList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                    End If
                End If

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

        'If L_ErrCode = enumEgswErrorCode.OneItemNotDeleted Then
        '    Dim da As New SqlDataAdapter

        '    Try
        '        cmd.CommandText = "sp_EgswItemGetNotDeleted"
        '        cmd.CommandType = CommandType.StoredProcedure
        '        cmd.Parameters.Clear()
        '        cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
        '        cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswBrand"

        '        L_dtList = New DataTable
        '        With da
        '            .SelectCommand = cmd
        '            L_dtList.BeginLoadData()
        '            .Fill(L_dtList)
        '            L_dtList.EndLoadData()
        '        End With
        '    Catch ex As Exception
        '        L_dtList.Dispose()
        '        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        '        cmd.Dispose()
        '        Throw New Exception(ex.Message, ex)
        '    End Try
        'End If

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Private Function ClearMarkings() As enumEgswErrorCode
        'Deactivate items that were not deleted by the Delete module
        If L_udtUser.Code <> -1 And L_intCodeSite <> -1 Then
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
        End If
    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get all UploadConfig Data.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object

        Return FetchList(-1)

    End Function

    ''' <summary>
    ''' Get a UploadConfig by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Brand to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object

        Return FetchList(lngCode)

    End Function

#End Region

#Region "Update Methods"

    ''' <summary>
    ''' Updates UploadConfig
    ''' </summary>
    ''' <param name="lngCode">The Code of the UploadConfig to be updated.</param>
    ''' <param name="udtUpload">One of the structUpload values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByRef lngCode As Int32, ByVal udtUpload As structUpload) As enumEgswErrorCode

        Return SaveIntoList(lngCode, udtUpload, CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge UploadConfig List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes an UploadConfig.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Brand to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Brand to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Brands specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of Brand Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

#End Region

#Region " Other Function "

    'Public Function GetOne(ByVal intCode As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataRow
    '    Dim tempFetchType As enumEgswFetchType = L_bytFetchType
    '    L_bytFetchType = enumEgswFetchType.DataSet
    '    Dim ds As DataSet = CType(GetList(intCode), DataSet)
    '    L_bytFetchType = tempFetchType

    '    Dim dt As DataTable = ds.Tables(2)
    '    If dt.DefaultView.Count = 0 Then Return Nothing

    '    Dim rw As DataRow = dt.Rows(0)
    '    If intCodeTrans > -1 Then
    '        Dim dtTrans As DataTable = ds.Tables(1)
    '        Dim rwTrans As DataRow

    '        If dtTrans.Select("CodeTrans=" & CStr(intCodeTrans)).Length > 0 Then
    '            rwTrans = dtTrans.Select("CodeTrans=" & CStr(intCodeTrans))(0)
    '            If Len(Trim(CStr(rwTrans("translationname")))) > 0 Then rw("name") = CStr(rwTrans("translationname"))
    '        End If
    '    End If
    '    Return rw
    'End Function

    '    Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
    '        If Trim(strName) = "" Then strName = "Not Defined"
    '        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
    '        L_bytFetchType = enumEgswFetchType.DataTable
    '        Dim dt As DataTable = CType(GetList(strName, enumDataListType.Merchandise, intCodeSite, intCodeTrans, 255), DataTable)
    '        L_bytFetchType = tempFetchType

    '        Dim intCode As Integer = -1
    '        Dim rw As DataRow = dt.Rows(0)

    '        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
    '        If Not blnCommitToDbase Then GoTo Done

    '        If intCode > -1 Then
    '            If CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
    '                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
    '                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswBrand, 1)
    '            End If
    '        Else
    '            Dim brand As structBrand
    '            brand.Type = enumDataListItemType.Merchandise
    '            brand.Code = intCode
    '            brand.Name = strName
    '            brand.IsGlobal = False

    '            Update(L_udtUser.Code, intCodeSite, intCode, brand)
    '        End If
    'Done:
    '        Return intCode
    '    End Function

#End Region




End Class



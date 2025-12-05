Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsProductPlace
'Decription         : Manages ProductionPlace Table
'Date Created       : 04.24.2008
'Author             : MRC
'Revision History   : 
#End Region

''' <summary>
''' Manages ProductionPlace Table
''' </summary>
''' <remarks></remarks>

Public Class clsProductionPlace

#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

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
            If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        ClearMarkings() 'items marked as not deleted
        MyBase.Finalize()
    End Sub

    Public ReadOnly Property AppType() As enumAppType
        Get
            AppType = L_AppType
        End Get
    End Property

    Public ReadOnly Property ItemsNotDeleted() As Object  'DataTable
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

    Public Property UserStruct() As structUser
        Get
            UserStruct = L_udtUser
        End Get
        Set(ByVal value As structUser)
            L_udtUser = UserStruct
            If L_udtUser.RoleLevelHighest < 0 Then Throw New Exception("User has an invalid RoleLevel.")
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

    Private Function FetchList(ByVal lngCodeTrans As Int32, Optional ByVal lngCode As Int32 = -1, Optional ByVal strName As String = "") As Object  'DataTable

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1


        If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
            lngCodeProperty = -1
        ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
            'lngCodeSite = L_udtUser.Site.Code
        ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
            lngCodeProperty = L_udtUser.Site.Group
        End If

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswProductionPlaceGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@vchName", SqlDbType.VarChar).Value = strName
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
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

    Public Function GetTranslationList() As Object 'DataTable
        'Get all
        Return FetchTranslationList(-1)
    End Function

    Public Function GetTranslationList(ByVal lngCodeTrans As Long) As Object 'DataTable
        'Filter by CodeTrans
        Return FetchTranslationList(lngCodeTrans)
    End Function

    Private Function FetchTranslationList(ByVal lngCodeTrans As Long) As DataTable

        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswProductionPlaceGetTranslationList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
            End With

            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With

        Catch ex As Exception
            dt.Dispose()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt

    End Function

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByRef lngCode As Int32, ByVal strName As String, ByVal TranMode As enumEgswTransactionMode) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd                
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswProductionPlaceUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 250).Value = strName                
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@retval", SqlDbType.Int)

                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                lngCode = CInt(.Parameters("@intCode").Value)
            End With

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodesite As Int32, _
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
                .CommandText = "sp_EgswTaxDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodesite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@retval", SqlDbType.Int)

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

        If L_ErrCode = enumEgswErrorCode.OneItemNotDeleted Then
            Dim da As New SqlDataAdapter

            Try
                cmd.CommandText = "sp_EgswItemGetNotDeleted"
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Clear()
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswTax"

                L_dtList = New DataTable
                With da
                    .SelectCommand = cmd
                    L_dtList.BeginLoadData()
                    .Fill(L_dtList)
                    L_dtList.EndLoadData()
                End With
            Catch ex As Exception
                L_dtList = Nothing
                If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
                cmd.Dispose()
                Throw New Exception(ex.Message, ex)
            End Try
        End If

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Public Function RemoveOne(ByVal lngCodeUser As Int32, ByVal lngCode As Int32) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswProductionPlaceDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@retval", SqlDbType.Int)

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

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode
    End Function

    Private Function ClearMarkings() As enumEgswErrorCode
        'Deactivate items that were not deleted by the Delete module
        If L_udtUser.Code <> -1 And L_lngCodeSite <> -1 Then
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
        End If
    End Function

#End Region

#Region "Get Methods"

    ''' <summary>
    ''' Get all Taxes with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object  'DataTable
        Return FetchList(2, -1, "")
    End Function

    ''' <summary>
    ''' Get a Tax by description.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>    
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal intCodeTrans As Integer) As Object

        Return FetchList(intCodeTrans, -1, strName)

    End Function


    ''' <summary>
    ''' Get a Tax by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Tax to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32, ByVal intCodeTrans As Integer) As Object  'DataTable

        Return FetchList(intCodeTrans, lngCode, "")

    End Function

    Public Overloads Function GetList(ByVal lngCode As Int32) As Object  'DataTable
        Return FetchList(0, lngCode, "")
    End Function

    Public Function GetName(ByVal intCode As Int32, ByVal intCodeTrans As Integer) As String
        Dim rw As DataRow = GetOne(intCode, intCodeTrans)
        If rw Is Nothing Then
            Return ""
        Else
            Return CStrDB(rw("Name"))
        End If
    End Function
#End Region

#Region "Update Methods"

    ''' <summary>
    ''' Updates the global status of a Tax.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Tax to be updated.</param>
    ''' <param name="IsGlobal">The global status of the Tax to be updated.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateGlobalStatus(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode


        Dim cmd As New SqlCommand
        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswTaxUpdateGlobal"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser

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

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Public Overloads Function Update(ByVal intCodeUser As Integer, ByRef lngCode As Int32, _
            ByVal strName As String, ByVal intCodeGroup As Integer, _
            ByVal arrTransCode() As String, ByVal arrTransName() As String) As enumEgswErrorCode


        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveIntoList(intCodeUser, lngCode, strName, CType(IIf(lngCode < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        If L_ErrCode = enumEgswErrorCode.OK Then
            Try
                Dim c As Int32 = arrTransCode.Length - 1
                Dim i As Int32
                For i = 0 To c
                    If IsNumeric(arrTransCode(i)) Then
                        L_ErrCode = UpdateTranslation(intCodeUser, lngCode, arrTransName(i), CInt(arrTransCode(i)))
                        If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                    End If
                Next
            Catch ex As Exception
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try

        End If

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    Public Function UpdateTranslation(ByVal lngCodeUser As Integer, ByVal lngCode As Int32, ByVal strName As String, ByVal lngCodeTrans As Int32) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Dim tntTranMode As Int32

        If lngCode < 1 Then
            tntTranMode = enumEgswTransactionMode.Add
        Else
            tntTranMode = enumEgswTransactionMode.Edit
        End If

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswProductionPlaceTransUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 250).Value = strName
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans                
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = tntTranMode
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If L_AppType = enumAppType.WebApp Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode
    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Tax List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a Tax.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Tax to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Tax to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Taxes specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <param name="strCodeList">The list of Tax Codes to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngcodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        'L_lngCodeSite = lngCodeSite
        Return RemoveFromList(lngCodeUser, lngcodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

#Region " Other Function "

    Public Function GetOne(ByVal intCode As Integer, ByVal intCodeTrans As Integer) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetList(intCode, intCodeTrans), DataSet)
        L_bytFetchType = tempFetchType

        Dim dt As DataTable = ds.Tables(1)
        If dt.DefaultView.Count = 0 Then Return Nothing
        Return dt.Rows(0)
    End Function

    Public Function GetCode(ByVal strName As String, ByVal intCodeTrans As Integer) As Integer
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(strName, intCodeTrans), DataTable)
        L_bytFetchType = tempFetchType

        Dim intCode As Integer = -1
        Dim rw As DataRow = dt.Rows(0)

        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
        Return intCode
    End Function

    Public Sub PopulateTax(ByVal cbo As Windows.Forms.ComboBox, ByVal intCodeSite As Int32)
        cbo.ResetText()
        cbo.Items.Clear()
        Dim dr As SqlDataReader = CType(GetList(255, intCodeSite), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                '                Debug.Print(dr.GetValue(dr.GetOrdinal("Name")).ToString)
                cbo.Items.Add(dr.GetValue(dr.GetOrdinal("Value")).ToString)
            Loop
        End If
        dr.Close()
        dr = Nothing
    End Sub

#End Region

#End Region

End Class

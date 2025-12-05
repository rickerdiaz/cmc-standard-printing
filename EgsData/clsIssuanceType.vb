Imports System.Data.SqlClient
Imports System.Data

Namespace FBControl
#Region "Class Header"
    'Name               : clsIssuanceType
    'Decription         : Manages IssuanceType Table
    'Date Created       : 27.09.2006
    'Author             : JHL
    'Revision History   :
#End Region

    ''' <summary>
    ''' Manages IssuanceType Table
    ''' </summary>
    ''' <remarks></remarks>

    Public Class clsIssuanceType

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

        Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
            ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, Optional ByVal strName As String = "", _
            Optional ByVal strNumberRef As String = "") As Object

            Dim ds As New DataSet
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable
            Dim dr As SqlDataReader
            Dim cmd As New SqlCommand
            Dim lngCodeProperty As Int32 = -1

            If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
                lngCodeProperty = -1
            ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
                lngCodeSite = L_udtUser.Site.Code
            ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
                lngCodeProperty = L_udtUser.Site.Group
            End If

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "ISS_GetList"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                    .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                    .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                    If strName.Trim <> "" Then _
                        .Parameters.Add("@vchName", SqlDbType.NVarChar, 30).Value = strName
                    If strName.Trim <> "" Then _
                        .Parameters.Add("@vchNumberRef", SqlDbType.NVarChar, 20).Value = strNumberRef
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
                        'dt.BeginLoadData()
                        .Fill(ds, "ItemList")
                        'dt.EndLoadData()
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

        Private Function FetchTranslationList(ByVal lngCodeTrans As Long) As DataTable

            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "ISS_GetTranslationList"
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
                cmd.Dispose()
                dt = Nothing
                Throw New Exception(ex.Message, ex)
            End Try

            cmd.Dispose()
            Return dt

        End Function

        Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByRef lngCode As Int32, ByVal udtIssuance As structIssuanceType, ByVal strCodeSiteList As String, _
            ByVal strCodeIssuanceList As String, ByVal TranMode As enumEgswTransactionMode, _
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

                    .CommandText = "ISS_Update"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = udtIssuance.Code
                    .Parameters.Add("@nvcName", SqlDbType.NVarChar, 30).Value = udtIssuance.Name
                    .Parameters.Add("@nvcNumberRef", SqlDbType.NVarChar, 20).Value = udtIssuance.NumberRef
                    .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 20).Value = udtIssuance.Description
                    .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtIssuance.IsGlobal
                    .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode

                    .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                    .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                    strCodeSiteList.Trim()
                    If strCodeSiteList <> "" Then
                        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
                            Return enumEgswErrorCode.InvalidCodeList
                        Else
                            .Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = strCodeSiteList
                        End If
                    End If

                    strCodeIssuanceList.Trim()
                    If strCodeIssuanceList <> "" Then
                        If Not (strCodeIssuanceList.StartsWith("(") And strCodeIssuanceList.EndsWith(")")) Then
                            Return enumEgswErrorCode.InvalidCodeList
                        Else
                            .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeIssuanceList
                        End If
                    End If

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
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "ISS_Delete"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                    .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                    .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                    .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                    strCodeList.Trim()
                    If strCodeList <> "" Then
                        If Not (strCodeList.StartsWith("(") And strCodeList.EndsWith(")")) Then
                            Return enumEgswErrorCode.InvalidCodeList
                        Else
                            .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                        End If
                    End If

                    If TranMode = enumEgswTransactionMode.ModifyStatus Then
                        .Parameters.Add("@bytStatus", SqlDbType.TinyInt).Value = bytStatus
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
                    cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswIssuance"

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

        Private Function ClearMarkings() As enumEgswErrorCode
            'Deactivate items that were not deleted by the Delete module
            If L_udtUser.Code <> -1 And L_intCodeSite <> -1 Then
                Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
            End If
        End Function

#End Region

#Region "Get Methods"
        ''' <summary>
        ''' Get all IssuanceTypes.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList() As Object

            Return FetchList(-1, -1, -1, 255)

        End Function

        ''' <summary>
        ''' Get a IssuanceType by Code.
        ''' </summary>
        ''' <param name="lngCode">The Code of the IssuanceType to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal lngCode As Int32) As Object

            Return FetchList(-1, lngCode, -1, 255)

        End Function

        ''' <summary>
        ''' Get all IssuanceTypes by Status.
        ''' </summary>
        ''' <param name="bytStatus">The status of the IssuanceTypes to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal bytStatus As Byte) As Object
            'Get all by Status
            Return FetchList(-1, -1, -1, bytStatus)

        End Function

        ''' <summary>
        ''' Get all IssuanceTypes with the list of Site names to which they are shared to.
        ''' </summary>
        ''' <param name="lngCodeTrans">The Code of the language translation.</param>
        ''' <param name="bytStatus">The status of the IssuanceTypes to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte) As Object

            Return FetchList(-1, -1, lngCodeTrans, bytStatus)

        End Function

        ''' <summary>
        ''' Get all IssuanceTypes shared to a specific site.
        ''' </summary>
        ''' <param name="lngCodeTrans">The Code of the language translation.</param>
        ''' <param name="bytStatus">The status of the IssuanceType to be fetched.</param>
        ''' <param name="lngCodeSite">The site to which the IssuanceType is shared.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, ByVal lngCodeSite As Int32) As Object

            Return FetchList(lngCodeSite, -1, lngCodeTrans, bytStatus)

        End Function

        ''' <summary>
        ''' Get all Translations.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTranslationList() As DataTable
            'Get all
            Return FetchTranslationList(-1)

        End Function

        ''' <summary>
        ''' Get a specific translation.
        ''' </summary>
        ''' <param name="lngCodeTrans">The Code of the language translation.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTranslationList(ByVal lngCodeTrans As Long) As DataTable
            'Filter by CodeTrans
            Return FetchTranslationList(lngCodeTrans)

        End Function

        ''' <summary>
        ''' Get an IssuanceType by Name w/in the codesite.
        ''' </summary>
        ''' <param name="strName">The name of the IssuanceType to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal strName As String, ByVal strNumberRef As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal bytStatus As Byte) As Object

            Return FetchList(intCodeSite, -1, intCodeTrans, bytStatus, strName, strNumberRef)

        End Function

#End Region

#Region "Update Methods"
        ''' <summary>
        ''' Standardize IssuanceTypes
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="eListeType">One of the enumDataListType values.</param>
        ''' <param name="eItemListType">One of the enumDataListType values.</param>
        ''' <param name="eFormat">One of the enumEgswStandardizationFormat values.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Standardize(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
            ByVal eItemListType As enumDataListItemType, ByVal eFormat As enumEgswStandardizationFormat) As enumEgswErrorCode

            Dim cmd As New SqlCommand

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "sp_EgswItemStandardizeAll"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                    .Parameters.Add("@tntFormat", SqlDbType.TinyInt).Value = eFormat
                    .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eListeType
                    .Parameters.Add("@tntListType", SqlDbType.TinyInt).Value = eItemListType

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

        ''' <summary>
        ''' Updates the global status of a IssuanceType.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCode">The Code of the IssuanceType to be updated.</param>
        ''' <param name="IsGlobal">The global status of the IssuanceType to be updated.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateGlobalStatus(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

            Dim cmd As New SqlCommand

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "ISS_UpdateGlobal"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
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

        ''' <summary>
        ''' Updates IssuanceType without sharing it to any sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the Brand to be updated.</param>
        ''' <param name="udtIssuance">One of the structIssuanceType values.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByRef lngCode As Int32, ByVal udtIssuance As structIssuanceType) As enumEgswErrorCode

            Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtIssuance, "", "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        End Function

        ''' <summary>
        ''' Updates IssuanceType sharing it to specified sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the item to be updated.</param>
        ''' <param name="udtIssuance">One of the structIssuanceType values.</param>
        ''' <param name="strCodeSiteList">The list of sites where IssuanceType will be shared.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
            ByVal udtIssuance As structIssuanceType, ByVal strCodeSiteList As String) As enumEgswErrorCode

            strCodeSiteList.Trim()
            If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
                Return enumEgswErrorCode.InvalidCodeList
            End If

            Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtIssuance, strCodeSiteList, "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        End Function

        ''' <summary>
        ''' Updates IssuanceType and its Translations and share it to specified sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the item to be updated.</param>
        ''' <param name="udtIssuance">One of the structIssuanceType values.</param>
        ''' <param name="strCodeSiteList">The list of sites where Brand will be shared.</param>
        ''' <param name="arrTransCode">The list of Translation Codes of the IssuanceType.</param>
        ''' <param name="arrTransName">The list of Translation Names of the IssuanceType.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
            ByVal udtIssuance As structIssuanceType, ByVal strCodeSiteList As String, _
            ByVal arrTransCode() As String, ByVal arrTransName() As String) As enumEgswErrorCode


            strCodeSiteList.Trim()
            If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
                Return enumEgswErrorCode.InvalidCodeList
            End If

            Dim t As SqlTransaction
            Dim cn As New SqlConnection(L_strCnn)

            cn.Open()
            t = cn.BeginTransaction()
            L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtIssuance, strCodeSiteList, "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
                 t)

            If L_ErrCode = enumEgswErrorCode.OK Then
                Try
                    Dim c As Int32 = arrTransCode.Length - 1
                    Dim i As Int32
                    Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                    For i = 0 To c
                        If IsNumeric(arrTransCode(i)) Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), 1, lngCodeSite, lngCodeUser, enumDataListType.IssuanceType)
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

        ''' <summary>
        ''' Updates IssuanceType and its Translations and share it to specified sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the item to be updated.</param>
        ''' <param name="udtIssuance">One of the structIssuanceType values.</param>
        ''' <param name="strCodeSiteList">The list of sites where IssuanceType will be shared.</param>
        ''' <param name="dtTranslations">The list of Translations of the IssuanceType.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
            ByVal udtIssuance As structIssuanceType, _
            ByVal strCodeSiteList As String, ByVal dtTranslations As DataTable) As enumEgswErrorCode

            strCodeSiteList.Trim()
            If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
                Return enumEgswErrorCode.InvalidCodeList
            End If

            Dim t As SqlTransaction
            Dim cn As New SqlConnection(L_strCnn)

            cn.Open()
            t = cn.BeginTransaction()
            L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtIssuance, strCodeSiteList, "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
                 t)

            If L_ErrCode > 0 Then
                Try
                    Dim rowX As DataRow
                    Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                    For Each rowX In dtTranslations.Rows
                        L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), 1, lngCodeSite, lngCodeUser, enumDataListType.IssuanceType)
                        If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
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

        ''' <summary>
        ''' Updatess IssuanceType's translations (multiple update)
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the item to be updated.</param>
        ''' <param name="eListeType">One of the enumDataListType values.</param>
        ''' <param name="dtTranslations">The list of Translations of the Brand.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
            ByVal eListeType As enumDataListType, ByVal dtTranslations As DataTable) As enumEgswErrorCode

            Dim t As SqlTransaction
            Dim cn As New SqlConnection(L_strCnn)

            cn.Open()
            t = cn.BeginTransaction()
            Try
                Dim rowX As DataRow
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For Each rowX In dtTranslations.Rows
                    L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), eListeType, lngCodeSite, lngCodeUser, enumDataListType.IssuanceType)
                    If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                Next

                oTrans = Nothing
            Catch
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try

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

        ''' <summary>
        ''' Updates a IssuanceType's translation (single update)
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the item to be updated.</param>
        ''' <param name="eListeType">One of the enumDataListType values.</param>
        ''' <param name="lngCodeTrans">The code of the IssuanceType's translation.</param>
        ''' <param name="strNameTrans">The name of the IssuanceType's translation.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, ByVal eListeType As enumDataListType, _
            ByVal lngCodeTrans As Int32, ByVal strNameTrans As String) As enumEgswErrorCode

            Dim t As SqlTransaction
            Dim cn As New SqlConnection(L_strCnn)

            cn.Open()
            t = cn.BeginTransaction()
            Try
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)
                L_ErrCode = oTrans.UpdateTranslation(lngCode, strNameTrans, lngCodeTrans, eListeType, lngCodeSite, lngCodeUser, enumDataListType.IssuanceType)

                oTrans = Nothing
            Catch
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try

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

        ''' <summary>
        ''' Merge IssuanceTypes
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="strCodeIssuanceList">The list of IssuanceType Codes to be merged.</param>
        ''' <param name="udtIssuance">IssuanceType info.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByVal strCodeIssuanceList As String, ByVal udtIssuance As structIssuanceType) As enumEgswErrorCode

            Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtIssuance, "", strCodeIssuanceList, enumEgswTransactionMode.MergeDelete)

        End Function

        ''' <summary>
        ''' Updates Status of the IssuanceTypes specified in the list (strCodeList).
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="strCodeList">The list of IssuanceType Codes to be updated.</param>
        ''' <param name="bytStatus">The Status of the IssuanceType.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte) As enumEgswErrorCode

            Return RemoveFromList(lngCodeUser, -1, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

        End Function

        ''' <summary>
        ''' Updates Status of a IssuanceType.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCode">The Code of the IssuanceType to be updated.</param>
        ''' <param name="IsGlobal">The Global Status of the IssuanceType.</param>
        ''' <param name="bytStatus">The Status of the IssuanceType.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
            ByVal bytStatus As Byte) As enumEgswErrorCode

            Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)

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
            ByVal lngCodeSite As Int32) As enumEgswErrorCode

            Dim cmd As New SqlCommand

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "ISS_MovePos"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                    .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                    .Parameters.Add("@bitMoveUp", SqlDbType.TinyInt).Value = flagMoveUp
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite

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

#Region "Remove Methods"
        ''' <summary>
        ''' Purge Brand List.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove() As enumEgswErrorCode

            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

        End Function

        ''' <summary>
        ''' Deletes an IssuanceType.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCode">The Code of the IssuanceType to be deleted.</param>
        ''' <param name="IsGlobal">The Global status of the IssuanceType to be deleted.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
                ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

            Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

        End Function

        ''' <summary>
        ''' Deletes IssuanceTypes specified in the list strCodeList.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="strCodeList">The list of IssuanceType Codes to be deleted.</param>
        ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

            L_udtUser.Code = lngCodeUser
            Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

        End Function

#End Region

#Region " Other Function "

        Public Function GetOne(ByVal intCode As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataRow
            Dim tempFetchType As enumEgswFetchType = L_bytFetchType
            L_bytFetchType = enumEgswFetchType.DataSet
            Dim ds As DataSet = CType(GetList(intCode), DataSet)
            L_bytFetchType = tempFetchType

            Dim dt As DataTable = ds.Tables(2)
            If dt.DefaultView.Count = 0 Then Return Nothing

            Dim rw As DataRow = dt.Rows(0)
            If intCodeTrans > -1 Then
                Dim dtTrans As DataTable = ds.Tables(1)
                Dim rwTrans As DataRow

                If dtTrans.Select("CodeTrans=" & CStr(intCodeTrans)).Length > 0 Then
                    rwTrans = dtTrans.Select("CodeTrans=" & CStr(intCodeTrans))(0)
                    If Len(Trim(CStr(rwTrans("translationname")))) > 0 Then rw("name") = CStr(rwTrans("translationname"))
                End If
            End If
            Return rw
        End Function

        Public Function GetCode(ByVal strName As String, ByVal strNumberRef As String, ByVal strDescription As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer

            If Trim(strName) = "" Then strName = "Not Defined"
            Dim tempFetchType As enumEgswFetchType = L_bytFetchType
            L_bytFetchType = enumEgswFetchType.DataTable
            Dim dt As DataTable = CType(GetList(strName, strNumberRef, intCodeSite, intCodeTrans, 255), DataTable)
            L_bytFetchType = tempFetchType

            Dim intCode As Integer = -1
            Dim rw As DataRow = dt.Rows(0)

            If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
            If Not blnCommitToDbase Then GoTo Done

            If intCode > -1 Then
                If CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
                    Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
                    cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswIssuance, 1)
                End If
            Else
                Dim IssuanceType As structIssuanceType
                IssuanceType.Code = intCode
                IssuanceType.Name = strName
                IssuanceType.NumberRef = strNumberRef
                IssuanceType.Description = strDescription
                IssuanceType.IsGlobal = False

                Update(L_udtUser.Code, intCodeSite, intCode, IssuanceType)
            End If
Done:
            Return intCode
        End Function

#End Region

    End Class
End Namespace



Imports System.Data.SqlClient
Imports System.Data
Imports EGSData.FBControl

Namespace FBControl
    ''' <summary>
    ''' Manages Location Table
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Public Class clsLocation
        Inherits clsDBRoutine


#Region "Class Header"
        'Name               : clsLocation
        'Decription         : Manages Location Table
        'Date Created       : 19.12.2005
        'Author             : VBV
        'Revision History   : {author} - {date} - {description}
        '                     {author} - {date} - {description}
        '
#End Region

#Region "Variable Declarations / Dependencies"
        'Private L_ErrCode As enumEgswErrorCode
        Private L_intCodeSite As Int32 = -1
        Private L_bytFetchType As enumEgswFetchType
        Private L_ErrCode As enumEgswErrorCode

        'Properties
        Private L_udtUser As structUser
        Private L_strCnn As String
        Private L_dtList As DataTable
#End Region

#Region "Class Functions and Properties"
        Public Sub New(ByVal udtUser As structUser, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

            Try
                If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
                L_udtUser = udtUser
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

        Public Property UserData() As structUser
            Get
                UserData = L_udtUser
            End Get
            Set(ByVal value As structUser)
                L_udtUser = UserData
                If L_udtUser.RoleLevelHighest < 0 Then Throw New Exception("User has an invalid RoleLevel.")
            End Set
        End Property
#End Region

#Region "Private Methods"
        Private Function FetchListMark(ByVal intCodeUser As Int32, ByVal intCodeSite As Int32, _
         ByVal EFilterMark As FilterMark, Optional ByVal intCodeTrans As Int32 = -1) As Object
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable
            Dim dr As SqlDataReader = Nothing
            Dim cmd As New SqlCommand
            'Dim intCodeProperty As Int32 = -1        

            If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
                'intCodeProperty = -1
            ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
                intCodeSite = L_udtUser.Site.Code
            ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
                'intCodeProperty = L_udtUser.Site.Group
            End If

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "LOC_GetListMark"
                    .CommandType = CommandType.StoredProcedure
                    .CommandTimeout = 600
                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                    .Parameters.Add("@flagGetMark", SqlDbType.TinyInt).Value = EFilterMark
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
                cmd.Connection.Open()
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

        Private Function FetchList(ByVal intCodeSite As Int32, ByVal intCode As Int32, _
            ByVal bytStatus As Byte) As Object

            Dim ds As New DataSet
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable
            Dim dr As SqlDataReader
            Dim cmd As New SqlCommand
            Dim intCodeProperty As Int32 = -1

            If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
                intCodeProperty = -1
            ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
                intCodeSite = L_udtUser.Site.Code
            ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
                intCodeProperty = L_udtUser.Site.Group
            End If

            Try
                With cmd
                    'If L_AppType = enumAppType.WebApp Then
                    '    .Connection = New SqlConnection(GetConnection("dsn"))
                    'Else
                    '    .Connection = L_Cnn
                    'End If                
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "LOC_GetList"
                    .CommandType = CommandType.StoredProcedure
                    .CommandTimeout = 600
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                    '.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = intCodeProperty
                    .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
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
                cmd.Connection.Close()
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

        Private Function SaveIntoList(ByVal intCodeUser As Int32, ByVal intCodeSite As Int32, _
            ByRef intCode As Int32, ByVal udtLoc As LocationData, ByVal strCodeSiteList As String, _
            ByVal strCodeLocationList As String, ByVal TranMode As enumEgswTransactionMode, _
            Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode

            Dim cmd As New SqlCommand

            Try
                With cmd
                    'If L_AppType = enumAppType.WebApp Then
                    '    .Connection = New SqlConnection(GetConnection("dsn"))
                    'Else
                    '    .Connection = L_Cnn
                    'End If                
                    If oTransaction Is Nothing Then
                        .Connection = New SqlConnection(L_strCnn)
                    Else
                        .Connection = oTransaction.Connection
                        .Transaction = oTransaction
                    End If

                    .CommandText = "LOC_Update"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                    .Parameters.Add("@nvcName", SqlDbType.NVarChar, 35).Value = udtLoc.Name
                    '.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtLoc.IsGlobal
                    .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode

                    .Parameters("@intCode").Direction = ParameterDirection.InputOutput

                    strCodeSiteList.Trim()
                    If strCodeSiteList <> "" Then
                        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
                            Return enumEgswErrorCode.InvalidCodeList
                        Else
                            .Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = strCodeSiteList
                        End If
                    End If

                    strCodeLocationList.Trim()
                    If strCodeLocationList <> "" Then
                        If Not (strCodeLocationList.StartsWith("(") And strCodeLocationList.EndsWith(")")) Then
                            Return enumEgswErrorCode.InvalidCodeList
                        Else
                            .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeLocationList
                        End If
                    End If

                    If oTransaction Is Nothing Then .Connection.Open()
                    .ExecuteNonQuery()
                    If oTransaction Is Nothing Then .Connection.Close()
                    L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    intCode = CInt(.Parameters("@intCode").Value)
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

        Private Function RemoveFromList(ByVal intCodeUser As Int32, ByVal intCodeSite As Int32, _
            ByVal intCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
             Optional ByVal bytStatus As Byte = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode
            'ByVal dataListItemType As enumDataListItemType,
            Dim cmd As New SqlCommand
            Dim intCodeProperty As Int32

            If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
                intCodeProperty = -1
            Else 'Unshare to ALL sites belonging to a property or Unshare to self
                intCodeProperty = L_udtUser.Site.Group
            End If

            'IsGlobal = L_udtUser.RoleLevelHighest = 0

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "LOC_Delete"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode

                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite

                    .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode

                    strCodeList.Trim()
                    If strCodeList <> "" Then
                        If Not (strCodeList.StartsWith("(") And strCodeList.EndsWith(")")) Then
                            Return enumEgswErrorCode.InvalidCodeList
                        Else
                            .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                        End If
                    End If

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
                    cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                    cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswLocation"

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

        Private Function ClearMarkings(ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode
            'Deactivate items that were not deleted by the Delete module
            If L_udtUser.Code <> -1 And L_intCodeSite <> -1 Then
                Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
            End If
        End Function
#End Region

#Region "Get Methods"
        ''' <summary>
        ''' Get all Locations.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>    
        Public Overloads Function GetList() As Object
            Return FetchList(-1, -1, 255)
        End Function

        Public Overloads Function GetList(ByVal bytStatus As Byte, ByVal intCodeSite As Int32) As Object
            Return FetchList(intCodeSite, -1, bytStatus)
        End Function

        ''' <summary>
        ''' Get a Location by Code.
        ''' </summary>
        ''' <param name="lngCode">The Code of the Location to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal lngCode As Int32) As Object

            Return FetchList(-1, lngCode, 255)

        End Function

        ''' <summary>
        ''' Get locations by site for markings.
        ''' </summary>
        ''' <param name="intCodeUser">The code of the user.</param>
        ''' <param name="intCodeSite">The code of the site.</param>    
        ''' <param name="EMarkFilter">One of FilterMark values.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetListMark(ByVal intCodeUser As Int32, ByVal intCodeSite As Integer, ByVal EMarkFilter As FilterMark) As Object
            'vbv 14.12.2005
            Return FetchListMark(intCodeUser, intCodeSite, EMarkFilter)

        End Function

        Public Function GetLocationCodeName() As Object
            Dim strCommandText As String = "[LOC_GETLISTCODENAME]"

            Dim arrParam(0) As SqlParameter
            arrParam(0) = New SqlParameter("@CodeSite", L_udtUser.Site.Code)

            Try
                Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
            Catch ex As Exception
                Throw ex
            End Try
        End Function

#End Region

#Region "Update Methods"

        ''' <summary>
        ''' Updates Location without sharing it to any sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the Location to be updated.</param>
        ''' <param name="udtLocation">One of the structLocation values.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByRef lngCode As Int32, ByVal udtLocation As LocationData) As enumEgswErrorCode

            Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtLocation, "", "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        End Function

        ''' <summary>
        ''' Merge Locations
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="strCodeLocationList">The list of Location Codes to be merged.</param>
        ''' <param name="udtLocation">Location info.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByVal strCodeLocationList As String, ByVal udtLocation As LocationData) As enumEgswErrorCode

            Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtLocation, "", strCodeLocationList, enumEgswTransactionMode.MergeDelete)

        End Function
#End Region



#Region "Remove Methods"
        ''' <summary>
        ''' Purge Location List.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove(ByVal lngCodeSite As Int32) As enumEgswErrorCode

            Return RemoveFromList(L_udtUser.Code, lngCodeSite, -1, False, enumEgswTransactionMode.Purge)

        End Function

        ''' <summary>
        ''' Deletes a Location.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCode">The Code of the Location to be deleted.</param>
        ''' <param name="IsGlobal">The Global status of the Location to be deleted.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
                ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

            Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

        End Function

        ''' <summary>
        ''' Deletes Locations specified in the list strCodeList.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="strCodeList">The list of Location Codes to be deleted.</param>
        ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

            L_udtUser.Code = lngCodeUser
            Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

        End Function

#End Region

    End Class
End Namespace
Imports System.Data.SqlClient
Imports System.Data



''' <summary>
''' Manages Unit Table
''' </summary>
''' <remarks></remarks>

Public Class clsUnit
#Region "Class Header"
    'Name               : clsUnit
    'Decription         : Manages Unit Table
    'Date Created       : 07.09.2005
    'Author             : VBV
    'Revision History   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
    '                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
    '                     VBV - 30.09.2005 - Added overload method GetList(ByVal strName As String, ByVal eType As enumDataListType)
    '                                      - Standardize method names
    '                     VBV - 29.12.2005 - Added FindUnitByName
#End Region
#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine
    'Inherits clsDBRoutine

    'Private L_Cnn As SqlConnection
    'Private cmd As New SqlCommand
    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1
    Private L_TranMode As enumEgswTransactionMode = enumEgswTransactionMode.None

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

    Public ReadOnly Property ItemsNotDeactivated() As Object  'DataTable
        Get
            ItemsNotDeactivated = L_dtList
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

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal eType As enumDataListItemType, _
        ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, _
        Optional ByVal strName As String = "", Optional ByVal blnSearchByName As Boolean = False, _
        Optional ByVal blnYield As Boolean = False, Optional ByVal blnIng As Boolean = False, _
        Optional ByVal blnStock As Boolean = False, Optional ByVal blnTranspo As Boolean = False, _
        Optional ByVal blnPack As Boolean = False) As Object  'DataTable


        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim intCodeProperty As Int32 = -1


        If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
            intCodeProperty = -1
        ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
            lngCodeSite = L_udtUser.Site.Code
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
                .CommandText = "sp_EgswUnitGetList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eType
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = intCodeProperty
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If blnSearchByName Then
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 32).Value = strName.Trim
                Else
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 32).Value = DBNull.Value
                End If
                .Parameters.Add("@IsYield", SqlDbType.Int).Value = blnYield
                If blnYield = False Then .Parameters("@IsYield").Value = DBNull.Value

                .Parameters.Add("@IsIngredient", SqlDbType.Int).Value = blnIng
                If blnIng = False Then .Parameters("@IsIngredient").Value = DBNull.Value

                .Parameters.Add("@IsStock", SqlDbType.Int).Value = blnStock
                If blnStock = False Then .Parameters("@IsStock").Value = DBNull.Value

                .Parameters.Add("@IsTransportation", SqlDbType.Int).Value = blnTranspo
                If blnTranspo = False Then .Parameters("@IsTransportation").Value = DBNull.Value

                .Parameters.Add("@IsPackaging", SqlDbType.Int).Value = blnPack
                If blnPack = False Then .Parameters("@IsPackaging").Value = DBNull.Value

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
        'AGL 2013.08.02
        If L_bytFetchType <> enumEgswFetchType.DataReader Then
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End If
        If L_bytFetchType = enumEgswFetchType.DataReader Then
            Return dr
        ElseIf L_bytFetchType = enumEgswFetchType.DataTable Then
            Return dt
        ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
            Return ds
        End If


    End Function

    Private Function FetchTranslationList(ByVal lngCodeTrans As Long) As Object  'DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUnitGetTranslationList"
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

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtUnit As structUnit, ByVal strCodeSiteList As String, _
        ByVal strCodeUnitList As String, ByVal TranMode As enumEgswTransactionMode, _
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
                .CommandText = "sp_EgswUnitUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtUnit.Code
                .Parameters.Add("@nvcNameDef", SqlDbType.NVarChar, 32).Value = IIf(udtUnit.NameDef Is Nothing, "", udtUnit.NameDef)
                .Parameters.Add("@nvcNamePlural", SqlDbType.NVarChar, 32).Value = IIf(udtUnit.NamePlural Is Nothing, "", udtUnit.NamePlural)
                .Parameters.Add("@nvcNameDisp", SqlDbType.NVarChar, 32).Value = IIf(udtUnit.NameDisplay Is Nothing, "", udtUnit.NameDisplay)
                .Parameters.Add("@nvcAutoConversion", SqlDbType.NVarChar, 500).Value = udtUnit.AutoConversion
                .Parameters.Add("@IsBasic", SqlDbType.Bit).Value = udtUnit.IsBasic
                .Parameters.Add("@IsStock", SqlDbType.Bit).Value = udtUnit.IsStock
                .Parameters.Add("@IsPackaging", SqlDbType.Bit).Value = udtUnit.IsPackaging
                .Parameters.Add("@IsTranspo", SqlDbType.Bit).Value = udtUnit.IsTransportation
                .Parameters.Add("@IsIngredient", SqlDbType.Bit).Value = udtUnit.IsIngredient
                .Parameters.Add("@IsYield", SqlDbType.Bit).Value = udtUnit.IsYield
                .Parameters.Add("@IsServing", SqlDbType.Bit).Value = udtUnit.IsServing
                .Parameters.Add("@fltFactor", SqlDbType.Float).Value = udtUnit.Factor
                .Parameters.Add("@intTypeMain", SqlDbType.Int).Value = udtUnit.TypeMain
                .Parameters.Add("@IsMetric", SqlDbType.Bit).Value = udtUnit.IsMetric
                .Parameters.Add("@nvcFormat", SqlDbType.NVarChar, 15).Value = IIf(udtUnit.Format Is Nothing, "", udtUnit.Format)
                .Parameters.Add("@IsAdded", SqlDbType.Bit).Value = udtUnit.IsAdded
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = udtUnit.Position
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtUnit.IsGlobal
                .Parameters.Add("@bitUseMakes", SqlDbType.Bit).Value = udtUnit.useMakes ' RDC 07.30.2013 : Use Makes [nth] [Unit] or  Ingredients for [nth] [Unit]
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

                strCodeUnitList.Trim()
                If strCodeUnitList <> "" Then
                    If Not (strCodeUnitList.StartsWith("(") And strCodeUnitList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeUnitList
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

    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngcodesite As Int32, _
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
                .CommandText = "sp_EgswUnitDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngcodesite
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

        If L_ErrCode = enumEgswErrorCode.OneItemNotDeleted Or _
           L_ErrCode = enumEgswErrorCode.OneItemNotDeactivated Then
            Dim da As New SqlDataAdapter

            Try
                If L_ErrCode = enumEgswErrorCode.OneItemNotDeleted Then
                    cmd.CommandText = "sp_EgswItemGetNotDeleted"
                ElseIf L_ErrCode = enumEgswErrorCode.OneItemNotDeactivated Then
                    cmd.CommandText = "sp_EgswItemGetNotDeactivated" '// DRR 11.09.2011 added
                End If

                'cmd.CommandText = "sp_EgswItemGetNotDeleted"
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Clear()
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswUnit"

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


    Public Function InsertUnitSharing(ByRef intUnitCode As Integer, ByVal strCodeSiteList As String, _
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
                .CommandText = "sp_InsertUnitSharing"
                .CommandType = CommandType.StoredProcedure
                '.Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUnit", SqlDbType.Int).Value = intUnitCode
                .Parameters.Add("@strCodeSiteList", SqlDbType.NVarChar, 500).Value = strCodeSiteList
                '.Parameters("@retval").Direction = ParameterDirection.ReturnValue

                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                'L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
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

    '// DRR 07.04.2012

    Public Function IsUnitInUsed(ByVal nCodeliste As Integer, ByVal nCodeUnit As Integer) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "select count(id) rn from egswdetails where Secondcode=@Codeliste and (Codeunit=@CodeUnit or Codeunitmetric=@CodeUnit or Codeunitimperial=@CodeUnit) "
            .CommandType = CommandType.Text
            .Parameters.Add("@Codeliste", SqlDbType.Int).Value = nCodeliste
            .Parameters.Add("@CodeUnit", SqlDbType.Int).Value = nCodeUnit            
        End With

        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With

        If dt.Rows(0).Item("rn") = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

    Private Function ClearMarkings() As enumEgswErrorCode
        'Deactivate items that were not deleted by the Delete module        
        If L_udtUser.Code <> -1 And L_TranMode = enumEgswTransactionMode.Delete Then
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
            L_TranMode = enumEgswTransactionMode.None
        End If
    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get all Units.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object  'DataTable

        Return FetchList(-1, -1, enumDataListItemType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get a unit by Name.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal eType As enumDataListItemType) As Object

        Return FetchList(-1, -1, eType, -1, 255, Trim(strName), True)

    End Function

    ''' <summary>
    ''' Get a Unit by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Unit to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object  'DataTable

        Return FetchList(-1, lngCode, enumDataListItemType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Units by Type.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType) As Object  'DataTable

        Return FetchList(-1, -1, eType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Units by Status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the Units to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Byte) As Object  'DataTable
        'Get all by Status
        Return FetchList(-1, -1, enumDataListItemType.NoType, -1, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Units with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Units to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte) As Object  'DataTable
        '
        Return FetchList(-1, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Units shared to a specific site.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListItemType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Unit to be fetched.</param>
    ''' <param name="lngCodeSite">The site to which the Unit is shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, ByVal lngCodeSite As Int32) As Object  'DataTable
        Return FetchList(lngCodeSite, -1, eType, lngCodeTrans, bytStatus)
    End Function

    ''' <summary>
    ''' Get all Units shared to a specific site, filtered by unit type.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListItemType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Unit to be fetched.</param>
    ''' <param name="lngCodeSite">The site to which the Unit is shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, ByVal lngCodeSite As Int32, _
        ByVal blnYield As Boolean, ByVal blnIng As Boolean, ByVal blnStock As Boolean, ByVal blnTranspo As Boolean, _
        ByVal blnPack As Boolean) As Object  'DataTable
        Return FetchList(lngCodeSite, -1, eType, lngCodeTrans, bytStatus, "", False, _
            blnYield, blnIng, blnStock, blnTranspo, blnPack)
    End Function

    ''' <summary>
    ''' Get a unit by Name w/in the codesite.
    ''' </summary>
    ''' <param name="strName">The name of the unit to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal intCodeTrans As Integer, ByVal bytStatus As Byte, ByVal intCodeSite As Integer, ByVal strName As String) As Object

        Return FetchList(intCodeSite, -1, eType, intCodeTrans, bytStatus, strName, True)

    End Function

    ''' <summary>
    ''' Get all Translations.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList() As Object  'DataTable

        Return FetchTranslationList(-1)

    End Function

    ''' <summary>
    ''' Get a specific translation.
    ''' </summary>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList(ByVal lngCodeTrans As Long) As Object  'DataTable

        Return FetchTranslationList(lngCodeTrans)

    End Function

    'AGL Merging 2012.09.04
    Public Function GetListUnitCodeName(ByVal intUnitType As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, Optional ByVal intCodeListe As Integer = -1) As Object 'DLSMay132009 'JTOC 30.08.2012 Added intCodeListe to parameter
        Dim strCommandText As String = "[GET_UnitCodeName]"
        'AGL Merging 2012.09.04
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intUnitType", intUnitType)
        arrParam(3) = New SqlParameter("@intCodeListe", intCodeListe) 'AGL Merging 2012.09.04
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetUnitList(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal intStatus As Integer = 255, Optional ByVal intCodeProperty As Integer = -1) As Object 'VRP 27.10.2008
        Dim strCommandText As String = "GET_UNITLIST"

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intStatus", intStatus)
        arrParam(3) = New SqlParameter("@intCodeProperty", intCodeProperty) 'MKAM 2014.10.27

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

	Public Function GetUnitTypeMain(ByVal intCode As Integer) As Object	'VRP 27.10.2008
		Dim strCommandText As String = "sp_GetTypeMain"

		Dim arrParam(1) As SqlParameter
		arrParam(0) = New SqlParameter("@intCode", intCode)


		Try
			Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
		Catch ex As Exception
			Throw ex
		End Try
	End Function

#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Standardize Units
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="eListeType">One of the enumDataListItemType values.</param>
    ''' <param name="eItemListType">One of the enumDataListItemType values.</param>
    ''' <param name="eFormat">One of the enumEgswStandardizationFormat values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Standardize(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListItemType, _
        ByVal eItemListType As enumDataListType, ByVal eFormat As enumEgswStandardizationFormat) As enumEgswErrorCode


        Dim cmd As New SqlCommand
        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
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
    ''' Updates the global status of a Unit.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Unit to be updated.</param>
    ''' <param name="IsGlobal">The global status of the Unit to be updated.</param>
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
                .CommandText = "sp_EgswUnitUpdateGlobal"
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

    ''' <summary>
    ''' Updates Unit without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Unit to be updated.</param>
    ''' <param name="udtUnit">One of the structUnit values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtUnit As structUnit) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtUnit, "(" & lngCodeSite & ")", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Unit sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtUnit">One of the structUnit values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Unit will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtUnit As structUnit, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtUnit, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Unit and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtUnit">One of the structUnit values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Unit will be shared.</param>
    ''' <param name="arrTransCode">The list of Translation Codes of the Unit.</param>
    ''' <param name="arrTransName">The list of Translation Names of the Unit.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtUnit As structUnit, ByVal strCodeSiteList As String, _
        ByVal arrTransCode() As String, ByVal arrTransName() As String, Optional ByVal arrTransName2() As String = Nothing, Optional ByVal arrPlural() As String = Nothing) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction

        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        't = cn.BeginTransaction() DRR 11.22.2011 commented due to timeout exception
        L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtUnit, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
             t)

        If L_ErrCode = enumEgswErrorCode.OK Then
            Try
                'Update Translations
                ' Dim arrTransCode() As String = (strTransCodeList.Split(CChar(",")))
                '  Dim arrTransName() As String = strTransNameList.Split(CChar(","))
                Dim c As Int32 = arrTransCode.Length - 1
                Dim i As Int32
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For i = 0 To c
                    If IsNumeric(arrTransCode(i)) Then
                        If Not arrTransName2 Is Nothing AndAlso Not arrPlural Is Nothing Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit, arrTransName2(i), arrPlural(i))
                        ElseIf Not arrTransName2 Is Nothing AndAlso arrPlural Is Nothing Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit, arrTransName2(i))
                        ElseIf arrTransName2 Is Nothing AndAlso Not arrPlural Is Nothing Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit, "", arrPlural(i))
                        Else
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit)
                        End If
                        If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                    End If
                Next

                oTrans = Nothing
            Catch ex As Exception
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try
        End If

        If L_ErrCode = enumEgswErrorCode.OK Then
            't.Commit() DRR 11.22.2011
        Else
            't.Rollback() DRR 11.22.2011
        End If
        't.Dispose() DRR 11.22.2011
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates Unit and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtUnit">One of the structUnit values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Unit will be shared.</param>
    ''' <param name="dtTranslations">The list of Translations of the Unit.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtUnit As structUnit, _
        ByVal strCodeSiteList As String, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction

        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtUnit, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
             t)

        If L_ErrCode > 0 Then
            Try
                Dim rowX As DataRow
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For Each rowX In dtTranslations.Rows
                    L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit)
                    If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                Next

                oTrans = Nothing
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
    ''' Updatess Unit's translations (multiple update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="dtTranslations">The list of Translations of the Unit.</param>
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
                L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), eListeType, lngCodeSite, lngCodeUser, enumDataListType.Unit)
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
    ''' Updates a Unit's translation (single update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The code of the Unit's translation.</param>
    ''' <param name="strNameTrans">The name of the Unit's translation.</param>
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
            L_ErrCode = oTrans.UpdateTranslation(lngCode, strNameTrans, lngCodeTrans, eListeType, lngCodeSite, lngCodeUser, enumDataListType.Unit)

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
    ''' Merge Units
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeUnitList">The list of Unit Codes to be merged.</param>
    ''' <param name="udtUnit">Unit info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal strCodeUnitList As String, ByVal udtUnit As structUnit, ByVal strCodeSiteList As String, _
        ByVal arrTransCode() As String, ByVal arrTransName() As String, _
        Optional ByVal arrTransName2() As String = Nothing, _
        Optional ByVal arrPlural() As String = Nothing) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtUnit, "", strCodeUnitList, enumEgswTransactionMode.MergeDelete)

        '// DRR 07.11.2012 Save unit translation
        If L_ErrCode = enumEgswErrorCode.OK Then
            Try
                'Update Translations
                ' Dim arrTransCode() As String = (strTransCodeList.Split(CChar(",")))
                '  Dim arrTransName() As String = strTransNameList.Split(CChar(","))
                Dim c As Int32 = arrTransCode.Length - 1
                Dim i As Int32
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For i = 0 To c
                    If IsNumeric(arrTransCode(i)) Then
                        If Not arrTransName2 Is Nothing AndAlso Not arrPlural Is Nothing Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit, arrTransName2(i), arrPlural(i))
                        ElseIf Not arrTransName2 Is Nothing AndAlso arrPlural Is Nothing Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit, arrTransName2(i))
                        ElseIf arrTransName2 Is Nothing AndAlso Not arrPlural Is Nothing Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit, "", arrPlural(i))
                        Else
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), enumDataListItemType.NoType, lngCodeSite, lngCodeUser, enumDataListType.Unit)
                        End If
                        If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                    End If
                Next

                oTrans = Nothing
            Catch ex As Exception
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try
        End If
        '//

        Return L_ErrCode
        'Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtUnit, "", strCodeUnitList, enumEgswTransactionMode.MergeDelete) DRR 07.11.2012 commented
    End Function

    ''' <summary>
    ''' Updates Status of the Units specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of Unit Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the Unit.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a Unit.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Unit to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the Unit.</param>
    ''' <param name="bytStatus">The Status of the Unit.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)

    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Unit List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode
        L_TranMode = enumEgswTransactionMode.Delete
        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a Unit.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Unit to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Unit to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode
        L_TranMode = enumEgswTransactionMode.Delete
        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Units specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <param name="strCodeList">The list of Unit Codes to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode
        L_TranMode = enumEgswTransactionMode.Delete
        L_udtUser.Code = lngCodeUser
        'L_lngCodeSite = lngCodeSite
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

    Public Function Deactivate(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.Deactivate, , strCodeList)
    End Function

#End Region

#Region " Other Functions "

    Public Function MergeSystem(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
       ByVal strCodeUnitList As String, ByVal udtUnit As structUnit) As enumEgswErrorCode
        Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtUnit, "", strCodeUnitList, enumEgswTransactionMode.MergeSystem)
    End Function

    Public Sub AppendConversion(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal strConversion As String)
        Dim udtUnit As New structUnit
        udtUnit.Code = lngCode
        udtUnit.AutoConversion = strConversion
        SaveIntoList(L_udtUser.Code, lngCodeSite, 0, udtUnit, "", "", enumEgswTransactionMode.UpdateAutoConversion)
    End Sub

    Public Function FindIngredientUnit(ByVal intDataOwner As Integer, ByVal strName As String, ByVal intCodeTrans As Integer) As Integer
        Dim fetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(enumDataListItemType.NoType, intCodeTrans, 255, intDataOwner), DataTable)
        L_bytFetchType = fetchType

        Dim dv As New DataView(dt)
        dv.RowFilter = "ingredient=1"
        Return FindUnit(dv, intDataOwner, strName, intCodeTrans)
    End Function

    Public Function FindYieldUnit(ByVal intDataOwner As Integer, ByVal strName As String, ByVal intCodeTrans As Integer) As Integer
        Dim fetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(enumDataListItemType.NoType, intCodeTrans, 255, intDataOwner), DataTable)
        L_bytFetchType = fetchType
        Dim dv As New DataView(dt)
        dv.RowFilter = "yield=1"
        Return FindUnit(dv, intDataOwner, strName, intCodeTrans)
    End Function

    Private Function FindUnit(ByVal dv As DataView, ByVal intDataOwner As Integer, ByVal strName As String, ByVal intCodeTrans As Integer) As Integer
        strName = strName.ToLower
        Dim hashUnitDef As Hashtable = FillHash("namedisplay", "code", dv)
        Dim hashUnitAC As Hashtable = FillHash("code", "autoconversion", dv)

        ' Find unit name in main table
        If hashUnitDef.Contains(strName) Then
            Return CInt(hashUnitDef(strName))
        End If


        ' Find unit in autoconversion
        Dim counter As Integer
        Dim counter2 As Integer
        Dim hashUnitACNew As New Hashtable
        Dim values() As String
        Dim value As String
        Dim arrCodes As New ArrayList
        Dim arrValue As New ArrayList
        Dim temp As String

        For counter = 0 To hashUnitAC.Count - 1
            arrValue.AddRange(hashUnitAC.Values)
            arrCodes.AddRange(hashUnitAC.Keys)
            For counter2 = 0 To arrValue.Count - 1
                temp = CStr(arrValue(counter2))
                values = temp.Split(CChar(" "))
                For Each value In values
                    If value = strName Then
                        Return CInt(arrCodes(counter2))
                    End If
                Next
            Next
        Next
        Return -1
    End Function

    Public Function GetOne(ByVal intCode As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataRow
        If intCode < 0 Then Return Nothing

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
                If Len(Trim(CStr(rwTrans("translationname")))) > 0 Then rw("namedisplay") = CStr(rwTrans("translationname"))
            End If
        End If
        Return rw
    End Function

    Public Function GetOneSimple(ByVal intCode As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataRow
        If intCode < 0 Then Return Nothing

        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim cmd As New SqlCommand

        Try
            With cmd
                Dim strSQL As String = ""

                strSQL &= "SELECT DISTINCT x.Code, CASE WHEN ISNULL(t.[Name],'') <> '' THEN  t.[Name] ELSE x.NameDisplay END AS NameDisplay, NameDef, "
                strSQL &= "		x.[Type], x.IsGlobal, x.AutoConversion, "
                strSQL &= "		x.Basic, x.Stock, x.Packaging, x.Transportation, x.Ingredient, x.Yield, x.Serving, "
                strSQL &= "		x.Factor, x.TypeMain, x.Metric, x.Format, x.Added "
                strSQL &= "FROM	EgswUnit x  "
                strSQL &= "		LEFT JOIN EgswItemTranslation t ON t.Code = x.Code AND t.CodeEgswTable = 135 "
                strSQL &= "			AND t.CodeTrans = @intCodeTrans  "
                strSQL &= "WHERE	X.Code = @intCode "

                .Connection = New SqlConnection(L_strCnn)
                .CommandText = strSQL
                .CommandType = CommandType.Text
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

            End With

            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With

        Catch ex As Exception
            dt.Dispose()
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()

        If dt.DefaultView.Count = 0 Then Return Nothing
        Dim rw As DataRow = dt.Rows(0)
        Return rw
    End Function

    Public Sub GetUnitServing(ByVal intCodeSite As Integer, ByRef intCodeUnit As Integer, ByRef strNameDef As String, _
        Optional ByVal intcodeTrans As Integer = -1, Optional ByRef strNameDisplay As String = "")
        Dim arrParam(4) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intUnitCode", SqlDbType.Int)
        arrParam(2) = New SqlParameter("@nvcNameDef", SqlDbType.NVarChar, 32)
        arrParam(3) = New SqlParameter("@intCodeTrans", SqlDbType.Int)
        arrParam(4) = New SqlParameter("@nvcNameDisplay", SqlDbType.NVarChar, 6)

        arrParam(1).Direction = ParameterDirection.Output
        arrParam(2).Direction = ParameterDirection.Output
        arrParam(4).Direction = ParameterDirection.Output

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetUnitServing", arrParam)
        Catch ex As Exception
            Throw ex
        End Try

        intCodeUnit = CInt(arrParam(1).Value)
        strNameDef = CStr(arrParam(2).Value)
        strNameDisplay = CStr(arrParam(4).Value)
    End Sub

    Public Function UpdateFlag(ByVal intCode As Integer, Optional ByVal blnYield As Boolean = False, Optional ByVal blnIngredient As Boolean = False) As enumEgswErrorCode
        Dim arrParam(3) As SqlParameter

        arrParam(0) = New SqlParameter("@IsYield", blnYield)
        arrParam(1) = New SqlParameter("@IsIngredient", blnIngredient)
        arrParam(2) = New SqlParameter("@intCode", intCode)
        arrParam(3) = New SqlParameter("@retval", SqlDbType.Int)

        arrParam(3).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswUnitUpdateFlag", arrParam)
            Return CType(arrParam(3).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal IsYield As Boolean, ByVal IsIngredient As Boolean, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
        'If Trim(strName) = "" Then strName = "Not Defined"
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(enumDataListItemType.Merchandise, intCodeTrans, 255, intCodeSite, strName), DataTable)
        L_bytFetchType = tempFetchType

        Dim intCode As Integer = -1
        Dim rw As DataRow = dt.Rows(0)

        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
        If Not blnCommitToDbase Then GoTo Done

        If intCode > -1 Then
            If IsDBNull(dt.Rows(0)("Status")) OrElse CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswUnit, 1)
            End If

            If IsIngredient AndAlso CInt(dt.Rows(0)("Ingredient")) <> 1 Then 'we need ingredient 
                UpdateFlag(intCode, False, True)
            End If

            If IsYield AndAlso CInt(dt.Rows(0)("Yield")) <> 1 Then 'we need yield
                UpdateFlag(intCode, True, False)
            End If
        Else
            Dim unit As structUnit
            unit.IsAdded = True
            unit.Code = intCode
            unit.NameDef = strName
            unit.NameDisplay = strName
            unit.AutoConversion = strName
            unit.Format = "0.00"
            unit.IsIngredient = IsIngredient
            unit.IsYield = IsYield
            unit.IsGlobal = False
            unit.Factor = 1
            Update(L_udtUser.Code, intCodeSite, intCode, unit)
        End If
Done:
        Return intCode
	End Function

    Public Sub ConvertPriceToMainUnit(ByRef intUnitCode As Integer, ByRef fltValue As Double, ByRef fltValue2 As Double, ByRef strFormat As String, ByRef strUnitName As String, ByRef fltFactor As Double, ByRef intUnitTypeMain As Integer, ByVal intcodeSite As Integer, ByVal intCodeTrans As Integer)
        Dim arrParam(8) As SqlParameter

        arrParam(0) = New SqlParameter("@fltValue", SqlDbType.Float)
        arrParam(1) = New SqlParameter("@fltValue2", SqlDbType.Float)
        arrParam(2) = New SqlParameter("@intCodeSite", intcodeSite)
        arrParam(3) = New SqlParameter("@intUnitCode", SqlDbType.Int)
        arrParam(4) = New SqlParameter("@nvcName", SqlDbType.NVarChar, 30)
        arrParam(5) = New SqlParameter("@nvcFormat", SqlDbType.NVarChar, 15)
        arrParam(6) = New SqlParameter("@fltUnitFactor", SqlDbType.Float)
        arrParam(7) = New SqlParameter("@intUnitTypeMain", SqlDbType.Int)
        arrParam(8) = New SqlParameter("@intCodeTrans", intCodeTrans)

        arrParam(0).Value = fltValue
        arrParam(1).Value = fltValue2
        arrParam(3).Value = intUnitCode

        arrParam(0).Direction = ParameterDirection.InputOutput
        arrParam(1).Direction = ParameterDirection.InputOutput
        arrParam(3).Direction = ParameterDirection.InputOutput
        arrParam(4).Direction = ParameterDirection.Output
        arrParam(5).Direction = ParameterDirection.Output
        arrParam(6).Direction = ParameterDirection.Output
        arrParam(7).Direction = ParameterDirection.Output

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetMainUnitConversion", arrParam)

            fltValue = CDbl(arrParam(0).Value)
            fltValue2 = CDbl(arrParam(1).Value)
            intUnitCode = CInt(arrParam(3).Value)
            strUnitName = CStr(arrParam(4).Value)
            strFormat = CStr(arrParam(5).Value)
            fltFactor = CDbl(arrParam(6).Value)
            intUnitTypeMain = CInt(arrParam(7).Value)

        Catch ex As Exception
            Throw New Exception
        End Try
	End Sub

	'JTOC 05.03.2013
	Public Function IsUnitActive(ByVal intCode As Integer) As Boolean
		Dim arrParam(1) As SqlParameter

		arrParam(0) = New SqlParameter("@intCode", SqlDbType.Float)
		arrParam(1) = New SqlParameter("@bitActive", SqlDbType.Float)


		arrParam(0).Value = intCode
		arrParam(1).Direction = ParameterDirection.Output

		Try
			ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswIsUnitActive", arrParam)
		Catch ex As Exception
			Throw New Exception
		End Try

		Return CBoolDB(arrParam(1).Value)

	End Function

    Public Function FindUnitByName(ByVal intCodeSite As Int32, ByVal strName As String) As Int32
        'VBV - 29.12.2005
        ' MISSING STORED PROCEDURE!
        Dim cmdX As New SqlCommand
        Dim intCode As Int32 = -1

        Try
            With cmdX
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "UNIT_GetCodeFromName"
                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = 0
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 500).Value = strName
                .Connection.Open()
                intCode = CInt(.ExecuteScalar)
                .Connection.Close()
            End With
        Catch ex As Exception
            cmdX.Dispose()
            Return -1
        End Try

        cmdX.Dispose()
        Return intCode
    End Function


    Public Function GetActiveUnitByType(ByVal intCodeSite As Integer, ByVal intType As Integer, ByVal intCodeTrans As Integer) As DataRow
        Dim strSQL As String = ""
        strSQL &= "SELECT u.code, u.format, "
        strSQL &= "CASE WHEN t.[Name] IS NULL OR LTRIM(RTRIM(t.[Name]))='' THEN x.NameDisplay ELSE t.[Name] END NameDisplay"
        strSQL &= "FROM egswUnit u "
        strSQL &= "INNER JOIN egswsharing s ON s.Code=u.Code AND s.[type]=1 AND s.CodeEgswTable=dbo.fn_egswGetTableId('egswUnit') "
        strSQL &= "LEFT JOIN EgswItemTranslation t ON t.Code = u.Code AND t.CodeEgswTable = dbo.fn_egswGetTableId('egswUnit') "
        strSQL &= "AND (t.CodeTrans = @CodeTrans OR t.CodeTrans IS NULL) "
        strSQL &= "WHERE u.type=@type AND s.CodeUsersharedto=@codesite AND s.status=1 "


    End Function


    Public Function ConvertToBestQuantity(ByVal intUnitCode As Integer, ByVal dblQty As Double, ByVal intCodeSite As Integer, Optional ByVal intCodeTrans As Integer = 1) As structUnit
        'RDTC March 24 2004: intMeteric parameter added with default value = 2
        '1 if we convert to best Metric unit, 0 if Imperial or User-Defined, 2 if any
        Dim udtUnit As structUnit
        Dim dblNewQty As Double
        Dim strFormat As String
        'Dim dblFactor As Double
        Dim strUnit As String
        Dim dblUnitfactor As Double
        Dim lngType As Integer

        dblUnitfactor = 1

        Dim rw As DataRow = GetOne(intUnitCode, intCodeTrans)
        'if rw is nothing then

        Dim rw2 As DataRow
        Select Case CType(rw("type"), Integer)
            Case 100 'Main Unit is KG
                'kg
                lngType = 100
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty >= 0.9995 Then GoTo Found

                'pounds
                lngType = 110
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'ounce
                lngType = 111
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'decagram
                lngType = 102
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'gram
                lngType = 101
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'miligram
                lngType = 104
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                GoTo NoActive

            Case 200 'Main unit is Liter
                'Gal
                lngType = 210
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'Ga
                lngType = 212
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'Liter
                lngType = 200
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty >= 0.9995 Then GoTo Found

                'Quart
                lngType = 220
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'Pint
                lngType = 230
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'Deciliter
                lngType = 201
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'Floz
                lngType = 240
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'Centiliter
                lngType = 202
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                'mililiter
                lngType = 203
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                GoTo NoActive

            Case 300 'Main unit is cup
                'Cup
                lngType = 300
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                lngType = 301
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                lngType = 302
                rw2 = GetActiveUnitByType(intCodeSite, lngType, intCodeTrans)
                If Not rw2 Is Nothing AndAlso dblQty / CDbl(rw2("factor")) >= 0.9995 Then GoTo Found

                GoTo NoActive

            Case Else 'No SubUnits : pc, bundle, each, bottle, can, drop, bag, pack, cornet
                GoTo NoActive

        End Select

Found:
        dblNewQty = dblQty / CDbl(rw2("factor"))
        strFormat = rw2("format").ToString
        strUnit = rw2("name").ToString
        dblUnitfactor = CDbl(rw2("factor"))
        GoTo Done

NoActive:
        dblNewQty = dblQty
        strFormat = rw("format").ToString
        strUnit = rw("name").ToString
        dblUnitfactor = CDbl(rw("factor"))
        GoTo Done

Done:
        udtUnit.NameDisplay = strUnit
        udtUnit.Format = strFormat
        udtUnit.Factor = dblNewQty 'reused factor value to place new quantity
    End Function


#End Region

#Region "Get Imperial & Metric units"
    '// DRR 12.23.2010 Added
    Public Function GetImperialMetricUnit(Optional ByVal IsMetric As Boolean = True) As Object
        Dim strSQL As String = "SELECT Code, NameDisplay FROM EgswUnit WHERE Metric=@Metric"
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@Metric", SqlDbType.Bit)
        arrParam(0).Value = IsMetric
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.Text, strSQL, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

    '// DRR 10.12.2011
    Public Function GetUnitList() As Object  'DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_GetUnit"
                .CommandType = CommandType.StoredProcedure
                '.Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
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

    Public Function GetUnitFormatMainFactor(ByVal lngCode As Long) As Object  'DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUnitGetFormatMainFactor"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
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

		Public Function sp_EgswGetPriceUnits(ByVal lngCode As Long) As Object  'DataTable

		Dim cmd As New SqlCommand
		Dim da As New SqlDataAdapter
		Dim dt As New DataTable

		Try
			With cmd
				.Connection = New SqlConnection(L_strCnn)
				.CommandText = "sp_EgswGetPriceUnits"
				.CommandType = CommandType.StoredProcedure
				.Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
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

    Public Function GetIngredientUnit(ByVal CodeUnit As Integer) As DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim strSQL As String = "SELECT Code,NameDisplay,Factor,TypeMain,[CodeTypeMain]=(SELECT Code FROM EgswUnit WHERE Type=U.TypeMain),[NameTypeMain]=(SELECT NameDisplay FROM EgswUnit WHERE Type=U.TypeMain) FROM EgswUnit U WHERE Code=@Code"
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = strSQL
                .CommandType = CommandType.Text
                .Parameters.Add("@Code", SqlDbType.Int).Value = CodeUnit

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
        Catch ex As Exception
            dt.Dispose()
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt
    End Function

    Public Function GetRecipeYield(ByVal CodeListe As Integer) As DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim strSQL As String = "SELECT Code,Number,Name,Yield,YieldUnit,SrQty,SrUnit,SrLevel FROM EgswListe WHERE Code=@CodeListe"
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = strSQL
                .CommandType = CommandType.Text
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = CodeListe

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
        Catch ex As Exception
            dt.Dispose()
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt
    End Function

End Class

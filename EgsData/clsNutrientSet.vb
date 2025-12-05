Imports System.Data.SqlClient
Imports System.Data
Public Class clsNutrientSet
    Private L_bytFetchType As enumEgswFetchType
    Private L_strCnn As String
    Private L_udtUser As structUser
    Private L_AppType As enumAppType
    Private L_ErrCode As enumEgswErrorCode  ' RDC 03.14.2013 - CWM-3300 Standardization Enhancement

    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
      Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader, _
      Optional ByVal CreateRecord As Boolean = False)

        Try
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn

        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Public Sub New(ByVal strCnn As String)
        L_strCnn = strCnn
        L_bytFetchType = enumEgswFetchType.DataTable
	End Sub

	Public Function GetNutrientCodeSet() As Object
		'JTOC 07.01.2013 Get default codeset
		Dim da As New SqlDataAdapter
		Dim dt As New DataTable
		Dim cmd As New SqlCommand
		Dim lngCodeProperty As Int32 = -1


		Try
			With cmd
				.Connection = New SqlConnection(L_strCnn)
				.CommandText = "SELECT TOP 1 Code FROM EgswNutrientSet"
				.CommandTimeout = 60000
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

		cmd.Dispose()

		Return dt


		Return Nothing
	End Function

    Public Function FetchList(Optional ByVal strName As String = "") As Object


        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1


        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "GET_NutrientSetList"
                .CommandType = CommandType.StoredProcedure
                If strName.Trim <> "" Then _
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 200).Value = strName
                .CommandTimeout = 60000
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
    Public Function FetchDetails(ByVal lngCode As Int32) As Object


        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1



        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "GET_NutrientSetDetails"
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 60000
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

    Public Function SaveNutrientSet(ByRef intID As Integer, ByVal intCodeSite As Integer, ByVal strName As String, ByVal blIsGlobal As Boolean, intNutrientDB As Integer) As Integer
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Dim intReturn As Integer
        Try
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "UPDATE_NutrientSet"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@Code", SqlDbType.Int, 4).Value = intID
                .Parameters.Add("@CodeSite", SqlDbType.Int, 4).Value = intCodeSite
                .Parameters.Add("@Name", SqlDbType.NVarChar).Value = strName
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = blIsGlobal
                .Parameters.Add("@NutrientDBCode", SqlDbType.Int).Value = intNutrientDB
                .Parameters("@Code").Direction = ParameterDirection.InputOutput
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()
                If intID = -2 Then
                    intID = CInt(.Parameters("@Code").Value)
                End If
                intReturn = CInt(.Parameters("@retval").Value)
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            Return intReturn
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function ProcessRemoveNutrientSet(ByVal codeLang As Integer, ByVal intcodeUser As Integer, ByVal intCodeSite As Integer, _
                                          ByVal intID As Integer, ByVal strName As String, _
                                          ByRef strOK As String, ByRef strInUsed As String, ByVal boolForceDel As Boolean, ByVal boolCheckOnly As Boolean) As String
        Dim eErrCode As enumEgswErrorCode
        eErrCode = RemoveFromList(intID, boolForceDel, boolCheckOnly)
        If eErrCode <> enumEgswErrorCode.OK And eErrCode <> enumEgswErrorCode.OneItemNotDeleted Then
            Dim strMsg As String = ""
            Dim cNotes As clsNotes = New clsNotes(eErrCode, strMsg, codeLang)
            ProcessRemoveNutrientSet = strMsg
            Exit Function
        End If
        If eErrCode = enumEgswErrorCode.OneItemNotDeleted Then
            strInUsed += "-" & strName & "\n"
        Else
            strOK += "-" & strName & "\n"
        End If
        Return ""
    End Function
    Private Function RemoveFromList(ByVal lngCode As Integer, ByVal boolForceDel As Boolean, ByVal boolCheckOnly As Boolean) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        'Dim lngCodeProperty As Int32

        'If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
        '    lngCodeProperty = -1
        'Else 'Unshare to ALL sites belonging to a property or Unshare to self
        '    lngCodeProperty = L_udtUser.Site.Group
        'End If

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "DELETE_NutrientSet"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@Code", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = boolForceDel
                .Parameters.Add("@CheckOnly", SqlDbType.Bit).Value = boolCheckOnly

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                RemoveFromList = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            RemoveFromList = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try


        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
    End Function

    ' RDC 03.14.2013 - CWM-3300 NutrientSet Standardization
    Public Function StandardizeNutrientSet(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
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

End Class

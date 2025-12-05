Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsTranslation
'Decription         : Manages Items Translation
'Date Created       : 07.09.2005
'Author             : VBV
'Revision History   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
'
#End Region

Public Class clsTranslation
    Inherits clsDBRoutine

    Private L_Cnn As SqlConnection
    'Private L_Cmd As New SqlCommand
    Private L_ErrCode As enumEgswErrorCode

    'Properties
    Private L_AppType As enumAppType
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType

#Region "Class Functions and Properties"
    Public Sub New(ByVal eAppType As enumAppType, ByVal strCnn As String, Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

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
            L_strCnn = strCnn
            L_bytFetchType = bytFetchType

        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Public ReadOnly Property ConnectionString() As String
        Get
            ConnectionString = L_strCnn
        End Get
    End Property

	'JTOC 09.04.2013 Get Language code equivalent of Translation
	Public Function GetCodeLang(ByVal intCode As Integer)

		Dim returnVal As Integer
		Dim cmd As New SqlCommand

		Try
			With cmd
				'If L_AppType = enumAppType.WebApp Then
				'    .Connection = New SqlConnection(GetConnection("dsn"))
				'Else
				'    .Connection = L_Cnn
				'End If


				.Connection = New SqlConnection(L_strCnn)

				.CommandText = "sp_EgswGetCodeDictionary"
				.CommandType = CommandType.StoredProcedure
				.Parameters.Add("@retval", SqlDbType.Int)
				.Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
				.Parameters("@retval").Direction = ParameterDirection.ReturnValue

				.Connection.Open()
				.ExecuteNonQuery()
				.Connection.Close()
				returnVal = .Parameters("@retval").Value
			End With

		Catch ex As Exception
			L_ErrCode = enumEgswErrorCode.GeneralError
			If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
			cmd.Dispose()
			Throw New Exception(ex.Message, ex)
		End Try

		If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
		cmd.Dispose()
		Return returnVal


	End Function

#End Region

#Region "Update Methods"
    Public Function UpdateTranslation(ByRef lngCode As Int32, ByVal strName As String, ByVal lngCodeTrans As Int32, _
        ByVal shrtType As Integer, ByVal lngCodeSite As Int32, ByVal lngCodeUser As Int32, _
        ByVal eListType As enumDataListType, Optional ByVal strName2 As String = "", Optional ByVal strPlural As String = "") As enumEgswErrorCode
        'MRC - 08.07.08 - For Ducasse customization.
        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswItemTranslationUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = strName ' JBB 09.01.2011 change 150 to 260
                .Parameters.Add("@nvcName2", SqlDbType.NVarChar, 150).Value = strName2 'MRC - 08.07.08 - For Ducasse customization.
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@tntListType", SqlDbType.Int).Value = eListType
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@tntType", SqlDbType.Int).Value = shrtType
                .Parameters.Add("@nvcPlural", SqlDbType.NVarChar, 150).Value = strPlural
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

End Class

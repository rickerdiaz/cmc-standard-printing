Imports System.Data.SqlClient

Public Class clsLinkFbRnPos

    Private L_strCnn As String

    Public Sub New(ByVal strCnn As String)
        L_strCnn = strCnn
    End Sub

    Public Function UpdateLinkFbRnPOS(ByRef intID As Integer, ByVal intTypeLink As Integer, _
        ByVal TranMode As enumEgswTransactionMode, ByVal intCodeProduct As Object, ByVal intCodeListe As Object, _
        ByVal intCodeSalesItem As Object, ByVal dblFactor As Double, ByVal intPriceUpdate As Integer, _
        ByVal intCodeUnitProduct As Integer, ByVal intCodeUnitListe As Integer, ByVal blnDefLink As Boolean, _
        Optional ByVal sqlTran As SqlTransaction = Nothing, _
        Optional ByVal blnIsDefault As Boolean = False, _
        Optional ByVal intCodeSite As Integer = -1) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        If sqlTran Is Nothing Then
            sqlCmd.Connection = New SqlConnection(L_strCnn)
            sqlCmd.Connection.Open()
        Else
            sqlCmd.Connection = sqlTran.Connection
            sqlCmd.Transaction = sqlTran
        End If

        Try
            With sqlCmd
                .CommandText = "sp_EgswLinkFbRnPOSUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intID", SqlDbType.Int).Value = intID
                .Parameters.Add("@TypeLink", SqlDbType.TinyInt).Value = intTypeLink
                .Parameters.Add("@TranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@CodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@CodeSalesItem", SqlDbType.Int).Value = intCodeSalesItem
                .Parameters.Add("@Factor", SqlDbType.Float).Value = dblFactor
                .Parameters.Add("@PriceUpdate", SqlDbType.TinyInt).Value = intPriceUpdate
                .Parameters.Add("@CodeUnitProduct", SqlDbType.Int).Value = intCodeUnitProduct
                .Parameters.Add("@CodeUnitListe", SqlDbType.Int).Value = intCodeUnitListe
                .Parameters.Add("@DefLink", SqlDbType.Bit).Value = blnDefLink
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@retval", SqlDbType.Int)

                .Parameters("@intID").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                'blnIsDefault   MRC - 07.18.08  -  flag for the default product of the site.
                .Parameters.Add("@bitIsDefault", SqlDbType.Bit).Value = blnIsDefault

                .ExecuteNonQuery()
                UpdateLinkFbRnPOS = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            End With
        Catch ex As Exception
            UpdateLinkFbRnPOS = enumEgswErrorCode.GeneralError
            If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            sqlCmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function


End Class

Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Public Class clsEncryptionMACTripleDES

    'Function to encode the string
    Function Encrypt(ByVal value As String, ByVal key As String) As String
        Dim mac3des As New System.Security.Cryptography.MACTripleDES
        Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key))

        If value Is Nothing Then
            value = ""
        End If
        Encrypt = Convert.ToBase64String( _
          System.Text.Encoding.UTF8.GetBytes(value)) & "-"c & _
          Convert.ToBase64String(mac3des.ComputeHash( _
          System.Text.Encoding.UTF8.GetBytes(value)))
        Return Encrypt
        '        Return Replace(Encrypt, "=", "¶¶¶")
        ''we should replace the equal sign, since it is blocked by .NET 
    End Function

    'Function to decode the string
    'Throws an exception if the data is corrupt
    Function Decrypt(ByVal value As String, ByVal key As String) As String
        Dim dataValue As String = ""
        Dim calcHash As String = ""
        Dim storedHash As String = ""

        Dim mac3des As New System.Security.Cryptography.MACTripleDES
        Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key))

        Try

            'value = Replace(value, "Â", "")
            'value = Replace(value, "¶¶¶", "=")

            dataValue = System.Text.Encoding.UTF8.GetString( _
                    Convert.FromBase64String(value.Split("-"c)(0)))
            storedHash = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Split("-"c)(1)))
            calcHash = System.Text.Encoding.UTF8.GetString( _
              mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dataValue)))

            If storedHash <> calcHash Then
                'Data was corrupted

                Throw New ArgumentException("Hash value does not match")
                'This error is immediately caught below
            End If
        Catch ex As Exception
            Throw New ArgumentException("Invalid TamperProofString")
        End Try

        Return dataValue

    End Function


End Class


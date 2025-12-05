Imports System
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports System.Numerics
Imports System.Globalization

''' <summary>
''' VB.NET translation of the provided crypto_service.ts
''' - AES-128-CBC with PKCS7 padding (Set/Set_Old, Get/Get_Old)
''' - HMAC-SHA256 -> hex -> BigInteger -> Base36 -> 25-char string (Get25CharString)
''' </summary>
Public Class CryptoService
    ' Provide the old environment secret here (used by Set_Old / Get_Old)
    Public Property EnvironmentSecret As String

    ' TS: set_old(value)
    Public Function Set_Old(value As String) As String
        If String.IsNullOrWhiteSpace(value) Then Return value
        If String.IsNullOrEmpty(EnvironmentSecret) Then
            Throw New InvalidOperationException("EnvironmentSecret is not set.")
        End If
        Dim key = Encoding.UTF8.GetBytes(EnvironmentSecret)
        Dim iv = Encoding.UTF8.GetBytes(EnvironmentSecret)
        ValidateKeyIv(key, iv)
        Return EncryptAesCbcPkcs7(value, key, iv)
    End Function

    ' TS: set(value) with fixed key/iv "4a2b4c99c0bc1012"
    Public Function [Set](value As String) As String
        If String.IsNullOrWhiteSpace(value) Then Return value
        Dim k As String = "4a2b4c99c0bc1012"
        Dim key = Encoding.UTF8.GetBytes(k)
        Dim iv = Encoding.UTF8.GetBytes(k)
        ValidateKeyIv(key, iv)
        Return EncryptAesCbcPkcs7(value, key, iv)
    End Function

    ' TS: get_old(value)
    Public Function Get_Old(encryptedBase64 As String) As String
        If String.IsNullOrEmpty(encryptedBase64) Then Return encryptedBase64
        If String.IsNullOrEmpty(EnvironmentSecret) Then
            Throw New InvalidOperationException("EnvironmentSecret is not set.")
        End If
        Dim key = Encoding.UTF8.GetBytes(EnvironmentSecret)
        Dim iv = Encoding.UTF8.GetBytes(EnvironmentSecret)
        ValidateKeyIv(key, iv)
        Return DecryptAesCbcPkcs7(encryptedBase64, key, iv)
    End Function

    ' TS: get(value) with fixed key/iv "4a2b4c99c0bc1012"
    Public Function [Get](encryptedBase64 As String) As String
        If String.IsNullOrEmpty(encryptedBase64) Then Return encryptedBase64
        Dim k As String = "4a2b4c99c0bc1012"
        Dim key = Encoding.UTF8.GetBytes(k)
        Dim iv = Encoding.UTF8.GetBytes(k)
        ValidateKeyIv(key, iv)
        Return DecryptAesCbcPkcs7(encryptedBase64, key, iv)
    End Function

    ' TS: get25CharString(code)
    ' HMAC-SHA256 with key "6v3b6d88c0bc1045", base36 uppercase, 25 chars (left-pad with '0' or take last 25)
    Public Shared Function Get25CharString(code As String) As String
        Dim hmacKey As Byte() = Encoding.UTF8.GetBytes("6v3b6d88c0bc1045")
        Dim data As Byte() = Encoding.UTF8.GetBytes(If(code, String.Empty))

        Dim hashBytes As Byte()
        Using hmac As New HMACSHA256(hmacKey)
            hashBytes = hmac.ComputeHash(data)
        End Using

        ' Convert to hex (lowercase is fine; NumberStyles.AllowHexSpecifier accepts both)
        Dim sb As New StringBuilder(hashBytes.Length * 2)
        For Each b In hashBytes
            sb.Append(b.ToString("x2"))
        Next
        Dim hex As String = sb.ToString()

        ' Convert hex string to BigInteger (treat as positive)
        Dim big As BigInteger = BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier)
        If big.Sign < 0 Then big = BigInteger.Negate(big)

        ' Convert to base36 uppercase
        Dim base36 As String = ToBase36(big)
        If base36.Length = 0 Then base36 = "0"

        ' Ensure exactly 25 characters: left-pad with 0s or take last 25
        If base36.Length < 25 Then
            Return base36.PadLeft(25, "0"c)
        Else
            Return base36.Substring(base36.Length - 25)
        End If
    End Function

    ' -------------------- Internals --------------------

    Private Shared Sub ValidateKeyIv(key As Byte(), iv As Byte())
        ' AES-128 requires 16-byte key/iv
        If key Is Nothing OrElse key.Length <> 16 Then
            Throw New ArgumentException("Key must be 16 bytes (AES-128).")
        End If
        If iv Is Nothing OrElse iv.Length <> 16 Then
            Throw New ArgumentException("IV must be 16 bytes (AES-128).")
        End If
    End Sub

    Private Shared Function EncryptAesCbcPkcs7(plainText As String, key As Byte(), iv As Byte()) As String
        Using aes As Aes = Aes.Create()
            aes.Key = key
            aes.IV = iv
            aes.Mode = CipherMode.CBC
            aes.Padding = PaddingMode.PKCS7

            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)
                    Dim inputBytes = Encoding.UTF8.GetBytes(plainText)
                    cs.Write(inputBytes, 0, inputBytes.Length)
                    cs.FlushFinalBlock()
                    Dim cipher As Byte() = ms.ToArray()
                    Return Convert.ToBase64String(cipher) ' CryptoJS AES.encrypt.toString() produces Base64 ciphertext
                End Using
            End Using
        End Using
    End Function

    Private Shared Function DecryptAesCbcPkcs7(cipherBase64 As String, key As Byte(), iv As Byte()) As String
        Dim cipherBytes = Convert.FromBase64String(cipherBase64)
        Using aes As Aes = Aes.Create()
            aes.Key = key
            aes.IV = iv
            aes.Mode = CipherMode.CBC
            aes.Padding = PaddingMode.PKCS7

            Using ms As New MemoryStream(cipherBytes)
                Using cs As New CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read)
                    Using sr As New StreamReader(cs, Encoding.UTF8)
                        Return sr.ReadToEnd()
                    End Using
                End Using
            End Using
        End Using
    End Function

    Private Shared Function ToBase36(value As BigInteger) As String
        If value.IsZero Then
            Return "0"
        End If

        Dim digits As Char() = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray()
        Dim sb As New StringBuilder()
        Dim v As System.Numerics.BigInteger = System.Numerics.BigInteger.Abs(value)

        While v > System.Numerics.BigInteger.Zero
            Dim remainder As System.Numerics.BigInteger
            v = System.Numerics.BigInteger.DivRem(v, 36, remainder)
            ' remainder is guaranteed to fit into Integer (0..35)
            Dim idx As Integer = CInt(remainder)
            sb.Insert(0, digits(idx))
        End While

        Return sb.ToString()
    End Function
End Class

' Optional helper extension (not required by the class above)
Module StringExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Function [Or](value As String, fallback As String) As String
        If String.IsNullOrEmpty(value) Then Return fallback
        Return value
    End Function
End Module
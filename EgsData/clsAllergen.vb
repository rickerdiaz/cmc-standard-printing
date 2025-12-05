Imports System.Data.SqlClient
Imports System.Data
Public Class clsAllergen
#Region "Class Header"
    'Name               : clsALlergen
    'Decription         : Manages Allergen Table
    'Date Created       : 2006.03.01
    'Author             : JRL
    'Revision History   : 
    '
#End Region
    Inherits clsDBRoutine

    'Private L_Cnn As SqlConnection
    'Private cmd As New SqlCommand
    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeUser As Int32 = -1
    Private L_lngCodeSite As Int32 = -1

    'Properties
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32
    Private L_udtUser As structUser


#Region "Class Functions and Properties"
    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
            L_udtUser = udtUser
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        '  ClearMarkings() 'items marked as not deleted
        MyBase.Finalize()
    End Sub

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

    Public Property FetchReturnType() As enumEgswFetchType
        Get
            FetchReturnType = L_bytFetchType
        End Get
        Set(ByVal value As enumEgswFetchType)
            L_bytFetchType = value
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

#Region "Save Methods"

    Public Overloads Function Update(ByVal udtAllergen As structAllergen, _
    ByVal arrTransCode() As String, ByVal arrTransName() As String) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        Try
            cn.Open()
            t = cn.BeginTransaction()
            L_ErrCode = SaveIntoList(udtAllergen, t)

            If L_ErrCode = enumEgswErrorCode.OK Then
                Try
                    'Update Translations
                    '      Dim arrTransCode() As String = (strTransCodeList.Split(CChar(",")))
                    '     Dim arrTransName() As String = strTransNameList.Split(CChar(","))
                    Dim c As Int32 = arrTransCode.Length - 1
                    Dim i As Int32
                    Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                    For i = 0 To c
                        If IsNumeric(arrTransCode(i)) Then
                            L_ErrCode = oTrans.UpdateTranslation(udtAllergen.Code, arrTransName(i), CInt(arrTransCode(i)), 2, L_udtUser.Site.Code, L_udtUser.Code, enumDataListType.Allergen)
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


        Catch ex As Exception
        Finally
            If cn.State <> ConnectionState.Closed Then cn.Close()
            cn.Dispose()
        End Try

        Return L_ErrCode
    End Function
    Private Function SaveIntoList(ByVal udtAllergen As structAllergen, _
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

                .CommandText = "ALLERGEN_UpdateItem"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@nvcAbbreviation", SqlDbType.NVarChar, 15).Value = udtAllergen.Abbreviation
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtAllergen.Code

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
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
#End Region

#Region "Get Methods"

    ''' <summary>
    ''' Returns list of Allergens per Site or property 
    ''' </summary>
    ''' <param name="intCodeSite">Used if Property is Disabled</param>
    ''' <param name="intCodeProperty">Used if Property is Enabled</param>
    ''' <param name="intCodeTrans"></param>
    ''' <param name="bytStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeSite As Integer, ByVal intCodeProperty As Integer, ByVal intCodeTrans As Integer, Optional ByVal bytStatus As Byte = 255) As Object
        Return Me.FetchList(-1, intCodeSite, bytStatus, intCodeTrans, intCodeProperty)
    End Function

    ''' <summary>
    ''' Get ALlergen Translation and Main Info
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCode As Integer) As Object
        Me.L_bytFetchType = enumEgswFetchType.DataSet
        Return Me.FetchList(intCode, -1, 255, 0, -1)
    End Function

    Public Function GetList(ByVal intCode As Integer, ByVal intFoodLaw As enumAllergenFoodLaw) As Object
        Me.L_bytFetchType = enumEgswFetchType.DataSet
        Return Me.FetchList(intCode, intFoodLaw)
    End Function

    Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer) As Integer
        If strName = "" Then Return -1
        Dim tmp_FetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataReader
        Dim dr As SqlDataReader = CType(FetchList(-1, intCodeSite, 1, 0, -1, strName), SqlDataReader)
        If dr.HasRows Then
            dr.Read()
            GetCode = CInt(CStrDB(dr("Code")))
        Else
            GetCode = -1
        End If
        dr.Close()
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="drAllergen"></param>
    ''' <param name="shtType">
    ''' 1: Get translation/abbreviation/FTBCode Translation/Name
    ''' 2: Get FTBCode Translation/Name
    ''' 3. Get Name
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetName(ByRef drAllergen As SqlDataReader, ByVal shtType As Short, ByVal cLang As clsEGSLanguage) As String
        GetName = ""
        Select Case shtType
            Case 1
                GoTo Abbrev
            Case 2
                GoTo FTBCode
            Case 3
                GoTo Name
        End Select


Abbrev:
        If CStrDB(drAllergen("abbreviation")) <> "" Then 'translation/abbreviation
            GetName = CStr(drAllergen.Item("abbreviation"))
        End If
        If GetName <> "" Then Exit Function
FTBCode:
        If CInt(drAllergen("FTBCode")) > 0 Then ' FTBCode Trnaslation
            GetName = cLang.GetString(CType(drAllergen.Item("FTBCode"), clsEGSLanguage.CodeType))
        End If
        If GetName <> "" Then Exit Function
Name:
        GetName = CStr(drAllergen.Item("Name"))
    End Function


    'LD20160929 GET Header of Allergen Used in Export
    Public Function GetAllergenDefList(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeUser As Integer = 0) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            If intCodeTrans < 0 Then intCodeTrans = 1
            Dim sqlQry As String = String.Format("exec GET_AllergenExportHeader {0},{1}", intCodeTrans.ToString(), intCodeUser.ToString())
            da = New SqlDataAdapter(sqlQry, cn)
            da.Fill(dt)
        Catch ex As Exception

        End Try

        Return dt

    End Function



#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Unassign item from site / property
    ''' </summary>
    ''' <param name="strCodeList"></param>
    ''' <param name="intCodeSite">Used if Property is disabled</param>
    ''' <param name="intCodeProperty">Used if Property is Enabled</param>
    ''' <param name="tranMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Remove(ByVal strCodeList As String, ByVal intCodeSite As Integer, ByVal intCodeProperty As Integer, ByVal tranMode As enumEgswTransactionMode) As enumEgswErrorCode
        Return Me.RemoveFromList(strCodeList, intCodeSite, intCodeProperty, enumEgswTransactionMode.Deactivate)
    End Function
#End Region

#Region "Private Methods"
    Private Function FetchList(ByVal CodeTrans As Integer, ByVal FoodLaw As enumAllergenFoodLaw) As Object
        Try
            Dim arrParam(1) As SqlParameter
            arrParam(0) = New SqlParameter("@CodeTrans", CodeTrans)
            arrParam(1) = New SqlParameter("@AllergenLaw", FoodLaw)
            Return Me.ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswGetAllergens", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function FetchList(ByVal intCode As Integer, ByVal intCodeSite As Integer, ByVal bytStatus As Byte, ByVal intCodeTrans As Integer, ByVal intCodeProperty As Integer, Optional ByVal strName As String = "") As Object
        Try

            Dim arrParam(5) As SqlParameter
            arrParam(0) = New SqlParameter("@intCode", intCode)
            arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
            arrParam(2) = New SqlParameter("@tntStatus", bytStatus)
            arrParam(3) = New SqlParameter("@intCodeTrans", intCodeTrans)
            arrParam(4) = New SqlParameter("@intCodeProperty", intCodeProperty)
            arrParam(5) = New SqlParameter("@nvcName", strName)
            Return Me.ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "ALLERGEN_GetList", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function RemoveFromList(ByVal strCodeList As String, ByVal intCodeSite As Integer, ByVal intCodeProperty As Integer, ByVal tranMode As enumEgswTransactionMode) As enumEgswErrorCode
        Try

            Dim arrParam(4) As SqlParameter
            arrParam(0) = New SqlParameter("@vchCodeList", strCodeList)
            arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
            arrParam(2) = New SqlParameter("@tntTranMode", tranMode)
            arrParam(3) = New SqlParameter("@intCodeProperty", intCodeProperty)
            arrParam(4) = New SqlParameter("@retval", SqlDbType.Int)
            arrParam(4).Direction = ParameterDirection.ReturnValue

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "ALLERGEN_RemoveItem", arrParam)
            Return CType(arrParam(4).Value, enumEgswErrorCode)

        Catch ex As Exception

            Return enumEgswErrorCode.GeneralError

        End Try
    End Function
#End Region


End Class

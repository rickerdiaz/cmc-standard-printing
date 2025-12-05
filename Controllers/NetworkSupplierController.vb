
Imports System
Imports System.Web
Imports System.Web.Http
Imports System.Collections.Generic
Imports System.Linq
Imports AttributeRouting.Web.Http
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports log4net
Imports System.Net
Imports System.Threading
Imports System.Drawing
Imports System.IO
Imports Newtonsoft.Json

'..LD20170609 Class controller for supplier network - (DynamoDB)

Namespace CalcmenuAPI
    Public Class NetworkSupplierController
        Inherits ApiController
        Private Shared ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        <System.Web.Http.HttpPost> <POST("api/NetworkSupplier/ManageUnit")> _
        Public Function ManageUnit(data As Models.SNUnitManage) As Models.SNUnitManage

            Dim xmldata As String = ""
            Using cmd As New SqlCommand

                ' Validation
                If data Is Nothing Then
                    Throw (New ArgumentNullException("merchandise data is empty"))
                End If


                If data.TransactionType = 2 Then
                    xmldata = String.Format("<units><unit1>{0}</unit1><unit2>{1}</unit2><unit3>{2}</unit3><unit4>{3}</unit4></units>" _
                                            , IIf(String.IsNullOrEmpty(data.Unit1), "", data.Unit1) _
                                            , IIf(String.IsNullOrEmpty(data.Unit2), "", data.Unit2) _
                                            , IIf(String.IsNullOrEmpty(data.Unit3), "", data.Unit3) _
                                            , IIf(String.IsNullOrEmpty(data.Unit4), "", data.Unit4))
                End If

                With cmd
                    Using cn = New SqlConnection(ConnectionString)
                        Try
                            .CommandTimeout = 120
                            .Connection = cn
                            .CommandText = "API_ManageUnit"
                            .CommandType = CommandType.StoredProcedure

                            .Parameters.Add("@transaction", SqlDbType.Int).Value = data.TransactionType
                            .Parameters.Add("@Unit", SqlDbType.VarChar, 50).Value = data.Unit1
                            .Parameters.Add("@xmldata", SqlDbType.Xml).Value = xmldata
                            .Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output
                            .Parameters.Add("@Unit1Code", SqlDbType.Int).Direction = ParameterDirection.Output
                            .Parameters.Add("@Unit2Code", SqlDbType.Int).Direction = ParameterDirection.Output
                            .Parameters.Add("@Unit3Code", SqlDbType.Int).Direction = ParameterDirection.Output
                            .Parameters.Add("@Unit4Code", SqlDbType.Int).Direction = ParameterDirection.Output
                            cn.Open()
                            .ExecuteNonQuery()

                            If data.TransactionType = 1 Then

                                data.Unit1Code = GetInt(.Parameters("@output").Value, -1)

                            ElseIf data.TransactionType = 2 Then

                                data.Unit1Code = GetInt(.Parameters("@Unit1Code").Value, -1)
                                data.Unit2Code = GetInt(.Parameters("@Unit2Code").Value, -1)
                                data.Unit3Code = GetInt(.Parameters("@Unit3Code").Value, -1)
                                data.Unit4Code = GetInt(.Parameters("@Unit4Code").Value, -1)

                            End If

                            data.ResponseCode = 200
                            data.ResponseMessage = ""

                        Catch ex As DatabaseException

                            data.ResponseCode = 500
                            data.ResponseMessage = ex.Message

                            Try

                            Catch

                            End Try

                        Catch ex As Exception
                            Console.WriteLine(ex.ToString())
                            data.ResponseCode = 500
                            data.ResponseMessage = ex.Message
                        Finally
                            If Not cn Is Nothing Then
                                cn.Close()
                                CType(cn, IDisposable).Dispose()
                            End If
                        End Try

                    End Using
                End With
            End Using


            Return data

        End Function

        <HttpGet> <[GET]("api/NetworkSupplier/DictionaryTranslationCode/{CodeTrans?}")> _
        Public Function getDictinaryTranslation(CodeTrans As String) As String

            Using cmd As New SqlCommand

                With cmd
                    Using cn = New SqlConnection(ConnectionString)
                        Try
                            .CommandTimeout = 120
                            .Connection = cn
                            .CommandText = "API_GetDictionaryCode"
                            .CommandType = CommandType.StoredProcedure
                            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = Integer.Parse(CodeTrans)
                            .Parameters.Add("@CodeDictionary", SqlDbType.Int).Direction = ParameterDirection.Output
                            cn.Open()
                            .ExecuteNonQuery()

                            Return GetStr(.Parameters("@CodeDictionary").Value, 0)

                        Catch ex As DatabaseException

                            Return "2"


                        Catch ex As Exception
                            Console.WriteLine(ex.ToString())
                            Return "2"
                        Finally
                            If Not cn Is Nothing Then
                                cn.Close()
                                CType(cn, IDisposable).Dispose()
                            End If
                        End Try

                    End Using
                End With
            End Using


        End Function

        <HttpPost> <POST("api/NetworkSupplier/update_supplier_merchandise")> _
        Public Function SaveSupplierNetworkMerchandise(data As Models.SupplierNetworkMerchandise) As Models.ResponseCallBack
            Dim response As New Models.ResponseCallBack
            Dim _trans As SqlTransaction = Nothing
            Dim resultCode As Integer = 0
            Dim pictures As String = ""
            Dim arrDAMCode As New ArrayList
            Try
                If DebugEnabled Then Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + JsonConvert.SerializeObject(data, Formatting.None, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore}))

                Using cmd As New SqlCommand

                    ' Validation
                    If data Is Nothing Then
                        Throw (New ArgumentNullException("merchandise data is empty"))
                    End If


                    With cmd
                        Using cn = New SqlConnection(ConnectionString)
                            Try
                                Dim intCodeListe As Integer = -1
                                Dim intCodeSetPrice As Integer = data.CodeSetPrice
                                Dim intCodeUser As Integer = GetInt(data.CodeUser) ' RBAJ-2013.12.19
                                ' Wait for 2 minutes for the query to execute before timing out
                                .CommandTimeout = 120

                                ''Main
                                .Connection = cn
                                .CommandText = "API_UpdateSupplierMerchandise"
                                .CommandType = CommandType.StoredProcedure

                                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite
                                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans
                                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = data.CodeSetPrice

                                .Parameters.Add("@EgsRef", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.EgsRef) = True, "", data.EgsRef.Trim())
                                .Parameters.Add("@Number", SqlDbType.NVarChar, 120).Value = If(IsNothing(data.Number) = True, "", data.Number.Trim())
                                .Parameters.Add("@Name", SqlDbType.NVarChar, 260).Value = If(IsNothing(data.Name) = True, "", data.Name.Trim())
                                .Parameters.Add("@OriginalName", SqlDbType.NVarChar, 260).Value = If(IsNothing(data.OriginalName) = True, "", data.OriginalName.Trim())

                                .Parameters.Add("@Category", SqlDbType.NVarChar, 200).Value = If(IsNothing(data.Category) = True, "", data.Category.Trim())
                                .Parameters.Add("@Brand", SqlDbType.NVarChar, 200).Value = If(IsNothing(data.Brand) = True, "", data.Brand.Trim())
                                .Parameters.Add("@Supplier", SqlDbType.NVarChar, 200).Value = If(IsNothing(data.Supplier) = True, "", data.Supplier.Trim())
                                .Parameters.Add("@Description", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.Description) = True, "", data.Description.Trim())
                                .Parameters.Add("@Declaration", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.Declaration) = True, "", data.Declaration.Trim())
                                .Parameters.Add("@Ingredients", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.Ingredients) = True, "", data.Ingredients.Trim())
                                .Parameters.Add("@Preparation", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.Preparation) = True, "", data.Preparation.Trim())
                                .Parameters.Add("@CookingTip", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.CookingTip) = True, "", data.CookingTip.Trim())
                                .Parameters.Add("@Refinement", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.Refinement) = True, "", data.Refinement.Trim())
                                .Parameters.Add("@Storage", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.Storage) = True, "", data.Storage.Trim())
                                .Parameters.Add("@Productivity", SqlDbType.VarChar, 2000).Value = If(IsNothing(data.Productivity) = True, "", data.Productivity.Trim())
                                .Parameters.Add("@Allergen", SqlDbType.NVarChar, 2000).Value = If(IsNothing(data.Allergen) = True, "", data.Allergen.Trim())
                                .Parameters.Add("@CountryOrigin", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.CountryOrigin) = True, "", data.CountryOrigin.Trim())
                                .Parameters.Add("@Attachment", SqlDbType.NVarChar, 200).Value = If(IsNothing(data.Attachment) = True, "", data.Attachment.Trim())
                                .Parameters.Add("@SpecificDetermination", SqlDbType.NVarChar, 300).Value = If(IsNothing(data.SpecificDetermination) = True, "", data.SpecificDetermination.Trim())
                                .Parameters.Add("@Barcode", SqlDbType.NVarChar, 60).Value = If(IsNothing(data.Barcode) = True, "", data.Barcode.Trim())
                                .Parameters.Add("@Price1", SqlDbType.Float).Value = If(IsNothing(data.Price1) = True, 0, data.Price1)
                                .Parameters.Add("@Price2", SqlDbType.Float).Value = If(IsNothing(data.Price2) = True, 0, data.Price2)
                                .Parameters.Add("@Price3", SqlDbType.Float).Value = If(IsNothing(data.Price3) = True, 0, data.Price3)
                                .Parameters.Add("@Price4", SqlDbType.Float).Value = If(IsNothing(data.Price4) = True, 0, data.Price4)
                                .Parameters.Add("@Ratio1", SqlDbType.Float).Value = If(IsNothing(data.Ratio1) = True, 0, data.Ratio1)
                                .Parameters.Add("@Ratio2", SqlDbType.Float).Value = If(IsNothing(data.Ratio2) = True, 0, data.Ratio2)
                                .Parameters.Add("@Ratio3", SqlDbType.Float).Value = If(IsNothing(data.Ratio3) = True, 0, data.Ratio3)
                                .Parameters.Add("@Unit1", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.Unit1) = True, "", data.Unit1.Trim())
                                .Parameters.Add("@Unit2", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.Unit2) = True, "", data.Unit2.Trim())
                                .Parameters.Add("@Unit3", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.Unit3) = True, "", data.Unit3.Trim())
                                .Parameters.Add("@Unit4", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.Unit4) = True, "", data.Unit4.Trim())
                                .Parameters.Add("@Tax", SqlDbType.NVarChar, 30).Value = If(IsNothing(data.Tax) = True, "", data.Tax.Trim())
                                .Parameters.Add("@Wastage", SqlDbType.Float).Value = 0
                                .Parameters.Add("@Wastage2", SqlDbType.Float).Value = 0
                                .Parameters.Add("@Wastage3", SqlDbType.Float).Value = 0
                                .Parameters.Add("@Wastage4", SqlDbType.Float).Value = 0
                                .Parameters.Add("@Wastage5", SqlDbType.Float).Value = 0

                                .Parameters.Add("@N1", SqlDbType.Float).Value = If(IsNothing(data.N1) = True, 0, data.N1)
                                .Parameters.Add("@N2", SqlDbType.Float).Value = If(IsNothing(data.N2) = True, 0, data.N2)
                                .Parameters.Add("@N3", SqlDbType.Float).Value = If(IsNothing(data.N3) = True, 0, data.N3)
                                .Parameters.Add("@N4", SqlDbType.Float).Value = If(IsNothing(data.N4) = True, 0, data.N4)
                                .Parameters.Add("@N5", SqlDbType.Float).Value = If(IsNothing(data.N5) = True, 0, data.N5)
                                .Parameters.Add("@N6", SqlDbType.Float).Value = If(IsNothing(data.N6) = True, 0, data.N6)
                                .Parameters.Add("@N7", SqlDbType.Float).Value = If(IsNothing(data.N7) = True, 0, data.N7)
                                .Parameters.Add("@N8", SqlDbType.Float).Value = If(IsNothing(data.N8) = True, 0, data.N8)
                                .Parameters.Add("@N9", SqlDbType.Float).Value = If(IsNothing(data.N9) = True, 0, data.N9)
                                .Parameters.Add("@N10", SqlDbType.Float).Value = If(IsNothing(data.N10) = True, 0, data.N10)
                                .Parameters.Add("@N11", SqlDbType.Float).Value = If(IsNothing(data.N11) = True, 0, data.N11)
                                .Parameters.Add("@N12", SqlDbType.Float).Value = If(IsNothing(data.N12) = True, 0, data.N12)
                                .Parameters.Add("@N13", SqlDbType.Float).Value = If(IsNothing(data.N13) = True, 0, data.N13)
                                .Parameters.Add("@N14", SqlDbType.Float).Value = If(IsNothing(data.N14) = True, 0, data.N14)
                                .Parameters.Add("@N15", SqlDbType.Float).Value = If(IsNothing(data.N15) = True, 0, data.N15)
                                .Parameters.Add("@N16", SqlDbType.Float).Value = If(IsNothing(data.N16) = True, 0, data.N16)
                                .Parameters.Add("@N17", SqlDbType.Float).Value = If(IsNothing(data.N17) = True, 0, data.N17)
                                .Parameters.Add("@N18", SqlDbType.Float).Value = If(IsNothing(data.N18) = True, 0, data.N18)
                                .Parameters.Add("@N19", SqlDbType.Float).Value = If(IsNothing(data.N19) = True, 0, data.N19)
                                .Parameters.Add("@N20", SqlDbType.Float).Value = If(IsNothing(data.N20) = True, 0, data.N20)
                                .Parameters.Add("@N21", SqlDbType.Float).Value = If(IsNothing(data.N21) = True, 0, data.N21)
                                .Parameters.Add("@N22", SqlDbType.Float).Value = If(IsNothing(data.N22) = True, 0, data.N22)
                                .Parameters.Add("@N23", SqlDbType.Float).Value = If(IsNothing(data.N23) = True, 0, data.N23)
                                .Parameters.Add("@N24", SqlDbType.Float).Value = If(IsNothing(data.N24) = True, 0, data.N24)
                                .Parameters.Add("@N25", SqlDbType.Float).Value = If(IsNothing(data.N25) = True, 0, data.N25)
                                .Parameters.Add("@N26", SqlDbType.Float).Value = If(IsNothing(data.N26) = True, 0, data.N26)
                                .Parameters.Add("@N27", SqlDbType.Float).Value = If(IsNothing(data.N27) = True, 0, data.N27)
                                .Parameters.Add("@N28", SqlDbType.Float).Value = If(IsNothing(data.N28) = True, 0, data.N28)
                                .Parameters.Add("@N29", SqlDbType.Float).Value = If(IsNothing(data.N29) = True, 0, data.N29)
                                .Parameters.Add("@N30", SqlDbType.Float).Value = If(IsNothing(data.N30) = True, 0, data.N30)
                                .Parameters.Add("@N31", SqlDbType.Float).Value = If(IsNothing(data.N31) = True, 0, data.N31)
                                .Parameters.Add("@N32", SqlDbType.Float).Value = If(IsNothing(data.N32) = True, 0, data.N32)
                                .Parameters.Add("@N33", SqlDbType.Float).Value = If(IsNothing(data.N33) = True, 0, data.N33)
                                .Parameters.Add("@N34", SqlDbType.Float).Value = If(IsNothing(data.N34) = True, 0, data.N34)
                                .Parameters.Add("@N35", SqlDbType.Float).Value = If(IsNothing(data.N35) = True, 0, data.N35)
                                .Parameters.Add("@N36", SqlDbType.Float).Value = If(IsNothing(data.N36) = True, 0, data.N36)
                                .Parameters.Add("@N37", SqlDbType.Float).Value = If(IsNothing(data.N36) = True, 0, data.N37)
                                .Parameters.Add("@N38", SqlDbType.Float).Value = If(IsNothing(data.N36) = True, 0, data.N38)
                                .Parameters.Add("@N39", SqlDbType.Float).Value = If(IsNothing(data.N36) = True, 0, data.n39)
                                .Parameters.Add("@N40", SqlDbType.Float).Value = If(IsNothing(data.N36) = True, 0, data.N40)
                                .Parameters.Add("@N41", SqlDbType.Float).Value = If(IsNothing(data.N36) = True, 0, data.N41)
                                .Parameters.Add("@N42", SqlDbType.Float).Value = If(IsNothing(data.N36) = True, 0, data.N42)

                                .Parameters.Add("@N1Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N1Name) = True, "", data.N1Name.Trim())
                                .Parameters.Add("@N2Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N2Name) = True, "", data.N2Name.Trim())
                                .Parameters.Add("@N3Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N3Name) = True, "", data.N3Name.Trim())
                                .Parameters.Add("@N4Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N4Name) = True, "", data.N4Name.Trim())
                                .Parameters.Add("@N5Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N5Name) = True, "", data.N5Name.Trim())
                                .Parameters.Add("@N6Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N6Name) = True, "", data.N6Name.Trim())
                                .Parameters.Add("@N7Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N7Name) = True, "", data.N7Name.Trim())
                                .Parameters.Add("@N8Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N8Name) = True, "", data.N8Name.Trim())
                                .Parameters.Add("@N9Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N9Name) = True, "", data.N9Name.Trim())
                                .Parameters.Add("@N10Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N10Name) = True, "", data.N10Name.Trim())
                                .Parameters.Add("@N11Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N11Name) = True, "", data.N11Name.Trim())
                                .Parameters.Add("@N12Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N12Name) = True, "", data.N12Name.Trim())
                                .Parameters.Add("@N13Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N13Name) = True, "", data.N13Name.Trim())
                                .Parameters.Add("@N14Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N14Name) = True, "", data.N14Name.Trim())
                                .Parameters.Add("@N15Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N15Name) = True, "", data.N15Name.Trim())
                                .Parameters.Add("@N16Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N16Name) = True, "", data.N16Name.Trim())
                                .Parameters.Add("@N17Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N17Name) = True, "", data.N17Name.Trim())
                                .Parameters.Add("@N18Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N18Name) = True, "", data.N18Name.Trim())
                                .Parameters.Add("@N19Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N19Name) = True, "", data.N19Name.Trim())
                                .Parameters.Add("@N20Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N20Name) = True, "", data.N20Name.Trim())
                                .Parameters.Add("@N21Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N21Name) = True, "", data.N21Name.Trim())
                                .Parameters.Add("@N22Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N22Name) = True, "", data.N22Name.Trim())
                                .Parameters.Add("@N23Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N23Name) = True, "", data.N23Name.Trim())
                                .Parameters.Add("@N24Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N24Name) = True, "", data.N24Name.Trim())
                                .Parameters.Add("@N25Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N25Name) = True, "", data.N25Name.Trim())
                                .Parameters.Add("@N26Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N26Name) = True, "", data.N26Name.Trim())
                                .Parameters.Add("@N27Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N27Name) = True, "", data.N27Name.Trim())
                                .Parameters.Add("@N28Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N28Name) = True, "", data.N28Name.Trim())
                                .Parameters.Add("@N29Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N29Name) = True, "", data.N29Name.Trim())
                                .Parameters.Add("@N30Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N30Name) = True, "", data.N30Name.Trim())
                                .Parameters.Add("@N31Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N31Name) = True, "", data.N31Name.Trim())
                                .Parameters.Add("@N32Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N32Name) = True, "", data.N32Name.Trim())
                                .Parameters.Add("@N33Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N33Name) = True, "", data.N33Name.Trim())
                                .Parameters.Add("@N34Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N34Name) = True, "", data.N34Name.Trim())
                                .Parameters.Add("@N35Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N35Name) = True, "", data.N35Name.Trim())
                                .Parameters.Add("@N36Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N36Name) = True, "", data.N36Name.Trim())

                                .Parameters.Add("@N37Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N36Name) = True, "", data.N37Name.Trim())
                                .Parameters.Add("@N38Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N36Name) = True, "", data.N38Name.Trim())
                                .Parameters.Add("@N39Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N36Name) = True, "", data.N39Name.Trim())
                                .Parameters.Add("@N40Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N36Name) = True, "", data.N40Name.Trim())
                                .Parameters.Add("@N41Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N36Name) = True, "", data.N41Name.Trim())
                                .Parameters.Add("@N42Name", SqlDbType.NVarChar, 100).Value = If(IsNothing(data.N36Name) = True, "", data.N42Name.Trim())

                                .Parameters.Add("@XMLTranslation", SqlDbType.Xml).Value = GenerateXMLTranslation(data.ProductTranslation)

                                ''Output Params
                                '.Parameters.Add("@intCodeListeNew", SqlDbType.Int).Direction = ParameterDirection.Output
                                .Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                                cn.Open()
                                _trans = cn.BeginTransaction
                                .Transaction = _trans
                                .ExecuteNonQuery()

                                'intCodeListe = GetInt(.Parameters("@intCodeListeNew").Value, -1)

                                resultCode = GetInt(.Parameters("@Return").Value, -1)
                                If resultCode > 0 Then
                                    _trans.Commit()
                                Else
                                    Throw New DatabaseException(String.Format("[{0}] Merchandise update failed", resultCode))
                                End If


                                ''### END: Long running queries here! ###
                                intCodeListe = resultCode
                                ' Done without errors
                                response.Code = 0
                                response.ReturnValue = intCodeListe
                                response.Message = Common.ReplaceSpecialCharacters(data.Name) & " successfully saved."
                                response.Status = True
                            Catch ex As DatabaseException
                                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Database error occured", ex)
                                Try
                                    _trans.Rollback()
                                    If Not _trans Is Nothing Then
                                        CType(_trans, IDisposable).Dispose()
                                    End If
                                Catch
                                End Try
                                If resultCode = 0 Then
                                    resultCode = 500
                                End If
                                response.Code = resultCode
                                response.Status = False
                                response.Message = "Save merchandise failed"
                                ' Common.SendEmail(Request.RequestUri.AbsoluteUri.ToString(), ex.Message.ToString(), ex.StackTrace.ToString(), "Merchandise")
                            Catch ex As Exception
                                Console.WriteLine(ex.ToString())
                            Finally
                                If Not cn Is Nothing Then
                                    cn.Close()
                                    CType(cn, IDisposable).Dispose()
                                End If
                            End Try

                        End Using
                    End With
                End Using
            Catch aex As ArgumentException
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Missing or invalid parameters", aex)
                response.Code = 400
                response.Message = "Missing or invalid parameters"
                response.Parameters = New List(Of Models.param) From {New Models.param With {.name = "data", .value = "Merchandise"}}
                'Common.SendEmail(Request.RequestUri.AbsoluteUri.ToString(), aex.Message.ToString(), aex.StackTrace.ToString(), "Merchandise")
            Catch hex As HttpResponseException
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", hex)
                'Common.SendEmail(Request.RequestUri.AbsoluteUri.ToString(), hex.Message.ToString(), hex.StackTrace.ToString(), "Merchandise")
                Throw hex
            Catch ex As Exception
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", ex)
                response.Status = False
                response.Message = "Unexpected error occured"
                response.Code = 500
                'Common.SendEmail(Request.RequestUri.AbsoluteUri.ToString(), ex.Message.ToString(), ex.StackTrace.ToString(), "Merchandise")
            End Try
            Return response

        End Function

        Function GenerateXMLTranslation(ProductTranslation As List(Of Models.SupplierProductTranslation)) As String

            Dim xmlresult As String = ""
            Try
                For Each data As Models.SupplierProductTranslation In ProductTranslation
                    xmlresult = xmlresult & String.Format("<Table>" & _
                                                            "<CodeTrans>{0}</CodeTrans>" & _
                                                            "<EgsRef>{1}</EgsRef>" & _
                                                            "<Name>{2}</Name>" & _
                                                            "<OriginalName>{3}</OriginalName>" & _
                                                            "<Category>{4}</Category>" & _
                                                            "<Brand>{5}</Brand>" & _
                                                            "<Description>{6}</Description>" & _
                                                            "<Declaration>{7}</Declaration>" & _
                                                            "<Ingredients>{8}</Ingredients>" & _
                                                            "<Preparation>{9}</Preparation>" & _
                                                            "<CookingTip>{10}</CookingTip>" & _
                                                            "<Refinement>{11}</Refinement>" & _
                                                            "<Storage>{12}</Storage>" & _
                                                            "<Productivity>{13}</Productivity>" & _
                                                            "<SpecificDetermination>{14}</SpecificDetermination>" & _
                                                            "</Table>" & Environment.NewLine,
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.CodeTrans), "", data.CodeTrans)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.EgsRef), "", data.EgsRef)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Name), "", data.Name)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.OriginalName), "", data.OriginalName)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Category), "", data.Category)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Brand), "", data.Brand)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Description), "", data.Description)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Declaration), "", data.Declaration)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Ingredients), "", data.Ingredients)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Preparation), "", data.Preparation)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.CookingTip), "", data.CookingTip)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Refinement), "", data.Refinement)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Storage), "", data.Storage)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.Productivity), "", data.Productivity)),
                                                            XMLEscape(IIf(String.IsNullOrEmpty(data.SpecificDetermination), "", data.SpecificDetermination))
                                                            )
                Next

                If xmlresult.Length > 0 Then
                    xmlresult = String.Format("<NewDataSet>{0}</NewDataSet>", xmlresult)
                End If

                Return xmlresult

            Catch ex As Exception
                Return ""
            End Try

        End Function

        Function XMLEscape(data As String) As String

            data = data.Replace("&", "&amp;")
            data = data.Replace("'", "&apos;")
            data = data.Replace("""", "&quot;")
            data = data.Replace("<", "&lt;")
            data = data.Replace(">", "&gt;")
            Return data

        End Function

        <HttpGet> <[GET]("/api/NetworkSupplier/ingredient/b/{codesite:int}/{codetrans:int}/{codesetprice:int}/{type:int}/{codeliste:int}")> _
        Public Function GetMerchandiseIngredient(codesite As Integer, _
                                 codetrans As Integer, _
                                 codesetprice As Integer, _
                                 type As Integer, _
                                 codeliste As Integer, _
                                 Optional name As String = "", _
                                 Optional skip As Integer = 0, _
                                 Optional take As Integer = 10, _
                                 Optional searchtype As Integer = 0, _
                                 Optional category As Integer = -1, _
                                 Optional sharetype As Integer = 0, _
                                 Optional namefilter As Integer = 0, _
                                 Optional isfulltext As Integer = 0, _
                                 Optional sortby As Integer = 1, _
                                 Optional status As Integer = -1
            ) As List(Of Models.Ingredient)

            Try

                ' Log method call
                If DebugEnabled Then LogMethodStart(System.Reflection.MethodBase.GetCurrentMethod(), _
                    codesite, codetrans, codesetprice, name, skip, take, searchtype, category, sharetype, namefilter, isfulltext, sortby)

                Dim ingredients As New List(Of Models.Ingredient)
                Dim totalCount As Integer

                Using cmd As New SqlCommand
                    cmd.CommandTimeout = 1200 'LLG 11.05.2015 added timeout in searching ingredients
                    With cmd
                        Using cn As New SqlConnection(ConnectionString)
                            Try
                                .Connection = cn
                                .CommandType = CommandType.StoredProcedure
                                '.CommandText = "[dbo].[API_GET_Ingredients]"
                                .CommandText = "[dbo].[API_GET_MerchandiseIngredient]" 'MKAM 2016.08.01
                                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = GetInt(codesite, -1)
                                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = GetInt(codetrans, -1)
                                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = GetInt(codesetprice, -1)
                                .Parameters.Add("@Name", SqlDbType.NVarChar, 250).Value = GetStr(name)
                                .Parameters.Add("@SearchType", SqlDbType.Int).Value = GetInt(searchtype, 0)
                                .Parameters.Add("@Category", SqlDbType.Int).Value = GetInt(category, -1)
                                .Parameters.Add("@ShareType", SqlDbType.Int).Value = GetInt(sharetype, 0)
                                .Parameters.Add("@pimstatus", SqlDbType.Int).Value = GetInt(status, -1)
                                .Parameters.Add("@intType", SqlDbType.Int).Value = GetInt(type, 0)

                                .Parameters.Add("@skip", SqlDbType.Int).Value = skip
                                .Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = take
                                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(codeliste, -1)

                                'AGL 2014.04.07
                                If isfulltext = 0 Then
                                    .Parameters.Add("@NameFilter", SqlDbType.Int).Value = GetInt(namefilter, 0)
                                End If

                                'AGL 2014.04.07
                                .Parameters.Add("@SortBy", SqlDbType.Int).Value = GetInt(sortby, 0)
                                .Parameters.Add("@IsFullText", SqlDbType.Int).Value = GetInt(isfulltext, 0)

                                cn.Open()

                                Using dr As SqlDataReader = cmd.ExecuteReader()
                                    If dr.HasRows Then
                                        While dr.Read
                                            totalCount = GetInt(dr("Total"))
                                        End While
                                    End If
                                    dr.NextResult()
                                    If dr.HasRows Then
                                        While dr.Read
                                            ingredients.Add(New Models.Ingredient With {
                                                            .CodeListe = GetInt(dr("CodeListe")), _
                                                            .Name = GetStr(dr("Name")), _
                                                            .Number = GetStr(dr("Number")), _
                                                            .CodeUser = GetInt(dr("CodeUser")), _
                                                            .Type = GetInt(dr("Type")), _
                                                            .Price = GetDbl(dr("Price")), _
                                                            .UnitName = GetStr(dr("UnitName")), _
                                                            .UnitMetric = GetStr(dr("UnitMetric")), _
                                                            .UnitImperial = GetStr(dr("UnitImperial")), _
                                                            .CodeUnit = GetInt(dr("CodeUnit")), _
                                                            .CodeUnitMetric = GetInt(dr("CodeUnitMetric")), _
                                                            .CodeUnitImperial = GetInt(dr("CodeUnitImperial")), _
                                                            .CategoryName = GetStr(dr("CategoryName")), _
                                                            .SourceName = GetStr(dr("SourceName")), _
                                                            .SupplierName = GetStr(dr("SupplierName")), _
                                                            .CodeBrand = GetInt(dr("CodeBrand")), _
                                                            .BrandName = GetStr(dr("BrandName")), _
                                                            .Wastage1 = GetInt(dr("Wastage1")), _
                                                            .Wastage2 = GetInt(dr("Wastage2")), _
                                                            .Wastage3 = GetInt(dr("Wastage3")), _
                                                            .Wastage4 = GetInt(dr("Wastage4")), _
                                                            .Wastage5 = GetInt(dr("Wastage5")), _
                                                            .WastageTotal = GetInt(dr("WastageTotal")), _
                                                            .Status = dr("Status"), _
                                                            .ImposedPrice = GetDbl(dr("ImposedPrice")), _
                                                            .Constant = GetInt(dr("Constant")), _
                                                            .Preparation = GetStr(dr("Preparation")), _
                                                            .Allprice = DisplayAllPrice(GetInt(dr("CodeListe")), codesetprice, type), _
                                                            .withTranslation = GetInt(dr("withTranslation")), _
                                                            .isLocked = GetBool(dr("isLocked")), _
                                                            .yieldIng = GetDbl(dr("Yield"))
                                                        })
                                        End While
                                    End If

                                    dr.Close()
                                End Using
                            Finally
                                If Not cn Is Nothing Then
                                    cn.Close()
                                    CType(cn, IDisposable).Dispose()
                                End If
                            End Try
                        End Using
                    End With
                End Using

                Return ingredients

            Catch aex As ArgumentException
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Missing or invalid parameters", aex)
                Throw New HttpResponseException(GenericErrorResponse("Request failed", HttpStatusCode.BadRequest, 440))
            Catch hex As HttpResponseException
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", hex)
                Throw hex
            Catch ex As Exception
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", ex)
                Throw New HttpResponseException(GenericErrorResponse("Request failed", HttpStatusCode.InternalServerError, 500))
            End Try
        End Function

        Private Function DisplayAllPrice(CodeListe As Integer, CodeSetPrice As Integer, type As Integer) As String
            Dim ingredients As New List(Of Models.Ingredient)
            Dim allprice As String = String.Empty
            Try

                Using cmd As New SqlCommand
                    With cmd
                        Using cn As New SqlConnection(ConnectionString)
                            Try
                                .Connection = cn
                                .CommandType = CommandType.StoredProcedure
                                .CommandText = "[dbo].[API_GET_IngredientAllSetPrice]"
                                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = CodeListe
                                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = CodeSetPrice
                                .Parameters.Add("@mainType", SqlDbType.Int).Value = GetInt(type, 0)
                                cn.Open()
                                Using dr As SqlDataReader = cmd.ExecuteReader()
                                    While dr.Read
                                        allprice += dr("Allprice") & ","
                                    End While
                                    dr.Close()
                                End Using
                            Finally
                                If Not cn Is Nothing Then
                                    cn.Close()
                                    CType(cn, IDisposable).Dispose()
                                End If
                            End Try
                        End Using
                    End With
                End Using

                Return allprice
            Catch ex As Exception
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", ex)
                Throw New HttpResponseException(GenericErrorResponse("Request failed", HttpStatusCode.InternalServerError, 500))
            End Try

        End Function

        <HttpGet> <[GET]("api/NetworkSupplier/GetSupplierNetworkStatus")> _
        Public Function getSupplierNetworkStatus() As String

            Using cmd As New SqlCommand

                With cmd
                    Using cn = New SqlConnection(ConnectionString)
                        Try
                            .CommandTimeout = 120
                            .Connection = cn
                            .CommandText = "API_GetSupplierNetworkStatus"
                            .CommandType = CommandType.StoredProcedure
                            .Parameters.Add("@Status", SqlDbType.Int).Direction = ParameterDirection.Output
                            cn.Open()
                            .ExecuteNonQuery()

                            Return GetStr(.Parameters("@Status").Value, 0)

                        Catch ex As DatabaseException

                            Return "0"


                        Catch ex As Exception
                            Console.WriteLine(ex.ToString())
                            Return "0"
                        Finally
                            If Not cn Is Nothing Then
                                cn.Close()
                                CType(cn, IDisposable).Dispose()
                            End If
                        End Try

                    End Using
                End With
            End Using


        End Function


    End Class

End Namespace

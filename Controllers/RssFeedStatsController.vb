Imports System
Imports System.Web
Imports System.Web.Http
Imports System.Collections.Generic
Imports System.Linq
Imports AttributeRouting.Web.Http
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports log4net
Imports System.Threading
Imports System.Drawing
Imports System.IO
Imports Newtonsoft.Json
Imports System.Net
Imports System.Text

Namespace CalcmenuAPI
    Public Class RssFeedStatsController
        Inherits ApiController
        Private Shared ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)


        <HttpGet> <[GET]("/api/rssfeedstats")> _
        Public Function GetRecipe() As Models.RSSFeedStats

            Dim RSSFeedStats As New Models.RSSFeedStats
            Try
                Using cmd As New SqlCommand()
                    With cmd
                        Using cn As New SqlConnection(ConnectionString)
                            Try
                                .Connection = cn
                                .CommandType = CommandType.StoredProcedure
                                .CommandText = "[dbo].[sp_GetRSSFeedsStats]"
                                cn.Open()

                                Using dr As SqlDataReader = cmd.ExecuteReader()
                                    '' Main
                                    If dr.HasRows Then
                                        While dr.Read
                                            RSSFeedStats.statsOverview = GetStr(dr("Overview"))
                                        End While
                                    End If

                                    '' Per Year
                                    dr.NextResult()
                                    If dr.HasRows Then
                                        Dim PerYear As New List(Of Models.StatsPerYear)
                                        While dr.Read
                                            Dim Years As New Models.StatsPerYear With {
                                                .DetailsPerYear = GetStr(dr("DetailsPerYear"))
                                                }
                                            PerYear.Add(Years)
                                        End While
                                        RSSFeedStats.statsPerYear = PerYear
                                    End If


                                    '' Per Month
                                    dr.NextResult()
                                    If dr.HasRows Then
                                        Dim PerMonth As New List(Of Models.StatsPerMonth)
                                        While dr.Read
                                            Dim Months As New Models.StatsPerMonth With {
                                                .DetailsPerMonth = GetStr(dr("DetailsPerMonth"))
                                                }
                                            PerMonth.Add(Months)
                                        End While
                                        RSSFeedStats.statsPerMonth = PerMonth
                                    End If


                                    '' Per Day
                                    dr.NextResult()
                                    If dr.HasRows Then
                                        Dim PerDay As New List(Of Models.StatsPerDay)
                                        While dr.Read
                                            Dim Days As New Models.StatsPerDay With {
                                                .DetailsPerDay = GetStr(dr("DetailsPerDay"))
                                                }
                                            PerDay.Add(Days)
                                        End While
                                        RSSFeedStats.statsPerDay = PerDay
                                    End If

                                    '' Per Hours
                                    dr.NextResult()
                                    If dr.HasRows Then
                                        Dim PerHour As New List(Of Models.StatsPerHours)
                                        While dr.Read
                                            Dim Days As New Models.StatsPerHours With {
                                                .DetailsPerHours = GetStr(dr("DetailsPerHour"))
                                                }
                                            PerHour.Add(Days)
                                        End While
                                        RSSFeedStats.statsPerHour = PerHour
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

            Return RSSFeedStats
        End Function


    End Class
End Namespace
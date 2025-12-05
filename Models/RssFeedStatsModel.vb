Namespace CalcmenuAPI.Models
    Public Class RSSFeedStats
        Private m_statsOverview As String
        Public Property statsOverview As String
            Get
                Return m_statsOverview
            End Get
            Set(value As String)
                m_statsOverview = value
            End Set
        End Property

        Private m_statsPerYear As List(Of Models.StatsPerYear)
        Public Property statsPerYear As List(Of Models.StatsPerYear)
            Get
                Return m_statsPerYear
            End Get
            Set(value As List(Of Models.StatsPerYear))
                m_statsPerYear = value
            End Set
        End Property

        Private m_statsPerMonth As List(Of Models.StatsPerMonth)
        Public Property statsPerMonth As List(Of Models.StatsPerMonth)
            Get
                Return m_statsPerMonth
            End Get
            Set(value As List(Of Models.StatsPerMonth))
                m_statsPerMonth = value
            End Set
        End Property

        Private m_statsPerDay As List(Of Models.StatsPerDay)
        Public Property statsPerDay As List(Of Models.StatsPerDay)
            Get
                Return m_statsPerDay
            End Get
            Set(value As List(Of Models.StatsPerDay))
                m_statsPerDay = value
            End Set
        End Property

        Private m_statsPerHour As List(Of Models.StatsPerHours)
        Public Property statsPerHour As List(Of Models.StatsPerHours)
            Get
                Return m_statsPerHour
            End Get
            Set(value As List(Of Models.StatsPerHours))
                m_statsPerHour = value
            End Set
        End Property

    End Class

    Public Class StatsPerYear
        Private m_DetailsPerYear As String
        Public Property DetailsPerYear As String
            Get
                Return m_DetailsPerYear
            End Get
            Set(value As String)
                m_DetailsPerYear = value
            End Set
        End Property
    End Class


    Public Class StatsPerMonth
        Private m_DetailsPerMonth As String
        Public Property DetailsPerMonth As String
            Get
                Return m_DetailsPerMonth
            End Get
            Set(value As String)
                m_DetailsPerMonth = value
            End Set
        End Property
    End Class

    Public Class StatsPerDay
        Private m_DetailsPerDay As String
        Public Property DetailsPerDay As String
            Get
                Return m_DetailsPerDay
            End Get
            Set(value As String)
                m_DetailsPerDay = value
            End Set
        End Property
    End Class

    Public Class StatsPerHours
        Private m_DetailsPerHours As String
        Public Property DetailsPerHours As String
            Get
                Return m_DetailsPerHours
            End Get
            Set(value As String)
                m_DetailsPerHours = value
            End Set
        End Property
    End Class
End Namespace
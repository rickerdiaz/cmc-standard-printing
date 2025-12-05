Namespace CalcmenuAPI.Models

    Public Class DigitalAssets
        Private m_id As Integer
        Public Property id As Integer
            Get
                Return m_id
            End Get
            Set(value As Integer)
                m_id = value
            End Set
        End Property
        Private m_ImageUrl As String
        Public Property ImageUrl As String
            Get
                Return m_ImageUrl
            End Get
            Set(value As String)
                m_ImageUrl = value
            End Set
        End Property
        Private m_codeuser As Integer
        Public Property CodeUser As Integer
            Get
                Return m_codeuser
            End Get
            Set(value As Integer)
                m_codeuser = value
            End Set
        End Property
        Private m_codesite As Integer
        Public Property CodeSite As Integer
            Get
                Return m_codesite
            End Get
            Set(value As Integer)
                m_codesite = value
            End Set
        End Property
        Private m_name As String
        Public Property Name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_filename As String
        Public Property FileName As String
            Get
                Return m_filename
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_mediatype As Integer
        Public Property MediaType As Integer
            Get
                Return m_mediatype
            End Get
            Set(value As Integer)
                m_mediatype = value
            End Set
        End Property
        Private m_extension As String
        Public Property Extension As String
            Get
                Return m_extension
            End Get
            Set(value As String)
                m_extension = value
            End Set
        End Property
        Private m_keyword As String
        Public Property Keyword As String
            Get
                Return m_keyword
            End Get
            Set(value As String)
                m_keyword = value
            End Set
        End Property
    End Class
    Public Class ResponseDigitalAssets

        Private m_Data As List(Of Models.DigitalAssets)
        Public Property Data As List(Of Models.DigitalAssets)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.DigitalAssets))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property Count As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.DigitalAssets), count As Integer)
            Me.Count = count
            Me.Data = data

        End Sub
    End Class
End Namespace
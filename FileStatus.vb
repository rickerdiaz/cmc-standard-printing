Imports System.IO

Public Class FilesStatus
    Public Const HandlerPath As String = "/"

    Public Property group() As String
        Get
            Return m_group
        End Get
        Set(value As String)
            m_group = value
        End Set
    End Property
    Private m_group As String
    Public Property name() As String
        Get
            Return m_name
        End Get
        Set(value As String)
            m_name = value
        End Set
    End Property
    Private m_name As String
    Public Property type() As String
        Get
            Return m_type
        End Get
        Set(value As String)
            m_type = value
        End Set
    End Property
    Private m_type As String
    Public Property size() As Integer
        Get
            Return m_size
        End Get
        Set(value As Integer)
            m_size = value
        End Set
    End Property
    Private m_size As Integer
    Public Property progress() As String
        Get
            Return m_progress
        End Get
        Set(value As String)
            m_progress = value
        End Set
    End Property
    Private m_progress As String
    Public Property url() As String
        Get
            Return m_url
        End Get
        Set(value As String)
            m_url = value
        End Set
    End Property
    Private m_url As String
    Public Property thumbnail_url() As String
        Get
            Return m_thumbnail_url
        End Get
        Set(value As String)
            m_thumbnail_url = value
        End Set
    End Property
    Private m_thumbnail_url As String
    Public Property delete_url() As String
        Get
            Return m_delete_url
        End Get
        Set(value As String)
            m_delete_url = value
        End Set
    End Property
    Private m_delete_url As String
    Public Property delete_type() As String
        Get
            Return m_delete_type
        End Get
        Set(value As String)
            m_delete_type = value
        End Set
    End Property
    Private m_delete_type As String
    Private m_result_code As Integer
    Public Property result_code() As Integer
        Get
            Return m_result_code
        End Get
        Set(value As Integer)
            m_result_code = value
        End Set
    End Property
    Public Property [error]() As String
        Get
            Return m_error
        End Get
        Set(value As String)
            m_error = value
        End Set
    End Property
    Private m_error As String

    Public Sub New()
    End Sub

    Public Sub New(fileInfo As FileInfo)
        SetValues(fileInfo.Name, CInt(fileInfo.Length))
    End Sub

    Public Sub New(fileName As String, fileLength As Integer)
        SetValues(fileName, fileLength)
    End Sub

    Private Sub SetValues(fileName As String, fileLength As Integer)
        name = fileName
        type = "image/png"
        size = fileLength
        progress = "1.0"
        url = Convert.ToString(HandlerPath & Convert.ToString("FileTransferHandler.ashx?f=")) & fileName
        thumbnail_url = Convert.ToString(HandlerPath & Convert.ToString("Thumbnail.ashx?f=")) & fileName
        delete_url = Convert.ToString(HandlerPath & Convert.ToString("FileTransferHandler.ashx?f=")) & fileName
        delete_type = "DELETE"
        result_code = 0
    End Sub
End Class

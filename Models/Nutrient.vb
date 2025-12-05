Namespace CalcmenuAPI.Models

    Public Class ResponseGenericNutrients

        Private m_Data As List(Of Models.NutrientList)
        Public Property data As List(Of Models.NutrientList)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.NutrientList))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property totalCount As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.NutrientList), _
            totalCount As Integer)
            Me.totalCount = totalCount
            Me.data = data

        End Sub
    End Class

    Public Class NutrientList
        Private m_Code As Integer
        Public Property code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_NDB_No As String
        Public Property NDB_No As String
            Get
                Return m_NDB_No
            End Get
            Set(value As String)
                m_NDB_No = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Number As String = ""
        Public Property Number As String
            Get
                Return m_Number
            End Get
            Set(value As String)
                m_Number = value
            End Set
        End Property
        Private m_status As Integer
        Public Property status() As Integer
            Get
                Return m_status
            End Get
            Set(ByVal value As Integer)
                m_status = value
            End Set
        End Property

        Private m_N1 As Double
        Public Property N1 As Double
            Get
                Return m_N1
            End Get
            Set(value As Double)
                m_N1 = value
            End Set
        End Property
        Private m_N1_1 As Double
        Public Property N1_1 As Double
            Get
                Return m_N1_1
            End Get
            Set(value As Double)
                m_N1_1 = value
            End Set
        End Property
        Private m_N1_2 As Double
        Public Property N1_2 As Double
            Get
                Return m_N1_2
            End Get
            Set(value As Double)
                m_N1_2 = value
            End Set
        End Property
        Private m_N2 As Double
        Public Property N2 As Double
            Get
                Return m_N2
            End Get
            Set(value As Double)
                m_N2 = value
            End Set
        End Property
        Private m_N3 As Double
        Public Property N3 As Double
            Get
                Return m_N3
            End Get
            Set(value As Double)
                m_N3 = value
            End Set
        End Property
        Private m_N4 As Double
        Public Property N4 As Double
            Get
                Return m_N4
            End Get
            Set(value As Double)
                m_N4 = value
            End Set
        End Property
        Private m_N5 As Double
        Public Property N5 As Double
            Get
                Return m_N5
            End Get
            Set(value As Double)
                m_N5 = value
            End Set
        End Property
        Private m_N6 As Double
        Public Property N6 As Double
            Get
                Return m_N6
            End Get
            Set(value As Double)
                m_N6 = value
            End Set
        End Property
        Private m_N7 As Double
        Public Property N7 As Double
            Get
                Return m_N7
            End Get
            Set(value As Double)
                m_N7 = value
            End Set
        End Property
        Private m_N8 As Double
        Public Property N8 As Double
            Get
                Return m_N8
            End Get
            Set(value As Double)
                m_N8 = value
            End Set
        End Property
        Private m_N9 As Double
        Public Property N9 As Double
            Get
                Return m_N9
            End Get
            Set(value As Double)
                m_N9 = value
            End Set
        End Property
        Private m_N10 As Double
        Public Property N10 As Double
            Get
                Return m_N10
            End Get
            Set(value As Double)
                m_N10 = value
            End Set
        End Property
        Private m_N11 As Double
        Public Property N11 As Double
            Get
                Return m_N11
            End Get
            Set(value As Double)
                m_N11 = value
            End Set
        End Property
        Private m_N12 As Double
        Public Property N12 As Double
            Get
                Return m_N12
            End Get
            Set(value As Double)
                m_N12 = value
            End Set
        End Property
        Private m_N13 As Double
        Public Property N13 As Double
            Get
                Return m_N13
            End Get
            Set(value As Double)
                m_N13 = value
            End Set
        End Property
        Private m_N14 As Double
        Public Property N14 As Double
            Get
                Return m_N14
            End Get
            Set(value As Double)
                m_N14 = value
            End Set
        End Property
        Private m_N15 As Double
        Public Property N15 As Double
            Get
                Return m_N15
            End Get
            Set(value As Double)
                m_N15 = value
            End Set
        End Property
        Private m_N16 As Double
        Public Property N16 As Double
            Get
                Return m_N16
            End Get
            Set(value As Double)
                m_N16 = value
            End Set
        End Property
        Private m_N17 As Double
        Public Property N17 As Double
            Get
                Return m_N17
            End Get
            Set(value As Double)
                m_N17 = value
            End Set
        End Property
        Private m_N18 As Double
        Public Property N18 As Double
            Get
                Return m_N18
            End Get
            Set(value As Double)
                m_N18 = value
            End Set
        End Property
        Private m_N19 As Double
        Public Property N19 As Double
            Get
                Return m_N19
            End Get
            Set(value As Double)
                m_N19 = value
            End Set
        End Property
        Private m_N20 As Double
        Public Property N20 As Double
            Get
                Return m_N20
            End Get
            Set(value As Double)
                m_N20 = value
            End Set
        End Property
        Private m_N21 As Double
        Public Property N21 As Double
            Get
                Return m_N21
            End Get
            Set(value As Double)
                m_N21 = value
            End Set
        End Property
        Private m_N22 As Double
        Public Property N22 As Double
            Get
                Return m_N22
            End Get
            Set(value As Double)
                m_N22 = value
            End Set
        End Property
        Private m_N23 As Double
        Public Property N23 As Double
            Get
                Return m_N23
            End Get
            Set(value As Double)
                m_N23 = value
            End Set
        End Property
        Private m_N24 As Double
        Public Property N24 As Double
            Get
                Return m_N24
            End Get
            Set(value As Double)
                m_N24 = value
            End Set
        End Property
        Private m_N25 As Double
        Public Property N25 As Double
            Get
                Return m_N25
            End Get
            Set(value As Double)
                m_N25 = value
            End Set
        End Property
        Private m_N26 As Double
        Public Property N26 As Double
            Get
                Return m_N26
            End Get
            Set(value As Double)
                m_N26 = value
            End Set
        End Property
        Private m_N27 As Double
        Public Property N27 As Double
            Get
                Return m_N27
            End Get
            Set(value As Double)
                m_N27 = value
            End Set
        End Property
        Private m_N28 As Double
        Public Property N28 As Double
            Get
                Return m_N28
            End Get
            Set(value As Double)
                m_N28 = value
            End Set
        End Property
        Private m_N29 As Double
        Public Property N29 As Double
            Get
                Return m_N29
            End Get
            Set(value As Double)
                m_N29 = value
            End Set
        End Property
        Private m_N30 As Double
        Public Property N30 As Double
            Get
                Return m_N30
            End Get
            Set(value As Double)
                m_N30 = value
            End Set
        End Property
        Private m_N31 As Double
        Public Property N31 As Double
            Get
                Return m_N31
            End Get
            Set(value As Double)
                m_N31 = value
            End Set
        End Property
        Private m_N32 As Double
        Public Property N32 As Double
            Get
                Return m_N32
            End Get
            Set(value As Double)
                m_N32 = value
            End Set
        End Property
        Private m_N33 As Double
        Public Property N33 As Double
            Get
                Return m_N33
            End Get
            Set(value As Double)
                m_N33 = value
            End Set
        End Property
        Private m_N34 As Double
        Public Property N34 As Double
            Get
                Return m_N34
            End Get
            Set(value As Double)
                m_N34 = value
            End Set
        End Property

        Private m_N35 As Double
        Public Property N35 As Double
            Get
                Return m_N35
            End Get
            Set(value As Double)
                m_N35 = value
            End Set
        End Property
        Private m_N36 As Double
        Public Property N36 As Double
            Get
                Return m_N36
            End Get
            Set(value As Double)
                m_N36 = value
            End Set
        End Property
        Private m_CheckoutUser As Integer
        Public Property CheckoutUser As Integer
            Get
                Return m_CheckoutUser
            End Get
            Set(value As Integer)
                m_CheckoutUser = value
            End Set
        End Property

        Private m_IsLocked As Boolean
        Public Property IsLocked As Boolean
            Get
                Return m_IsLocked
            End Get
            Set(value As Boolean)
                m_IsLocked = value
            End Set
        End Property
        Private m_pimstatus As Integer
        Public Property pimstatus As Integer
            Get
                Return m_pimstatus
            End Get
            Set(ByVal value As Integer)
                m_pimstatus = value
            End Set
        End Property

    End Class

    Public Class NutrientListData
        Private m_Data As List(Of Models.NutrientList)
        Public Property data As List(Of Models.NutrientList)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.NutrientList))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property totalCount As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.NutrientList), _
            totalCount As Integer)
            Me.totalCount = totalCount
            Me.data = data
        End Sub
    End Class
End Namespace
Namespace CalcmenuAPI.Models

    Public Class Workflow
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
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

        Private m_CodeTaskWorkflow As Integer
        Public Property CodeTaskWorkflow As Integer
            Get
                Return m_CodeTaskWorkflow
            End Get
            Set(value As Integer)
                m_CodeTaskWorkflow = value
            End Set
        End Property

        Private m_TaskName As String
        Public Property TaskName As String
            Get
                Return m_TaskName
            End Get
            Set(value As String)
                m_TaskName = value
            End Set
        End Property

        Private m_Duration As String
        Public Property Duration As String
            Get
                Return m_Duration
            End Get
            Set(value As String)
                m_Duration = value
            End Set
        End Property

        Private m_User As String
        Public Property User As String
            Get
                Return m_User
            End Get
            Set(value As String)
                m_User = value
            End Set
        End Property

        Private m_WorkflowName As String
        Public Property WorkflowName As String
            Get
                Return m_WorkflowName
            End Get
            Set(value As String)
                m_WorkflowName = value
            End Set
        End Property

        Private m_Codetask As Integer
        Public Property CodeTask As Integer
            Get
                Return m_Codetask
            End Get
            Set(value As Integer)
                m_Codetask = value
            End Set
        End Property

        Private m_Archive As Boolean
        Public Property [Archive] As Boolean
            Get
                Return m_Archive
            End Get
            Set(value As Boolean)
                m_Archive = value
            End Set
        End Property

        Private m_Status As String
        Private Property Status As String
            Get
                Return m_Status
            End Get
            Set(value As String)
                m_Status = value
            End Set
        End Property
    End Class
    Public Class WorkflowData
        Private m_Info As Models.Workflow
        Public Property Info As Models.Workflow
            Get
                Return m_Info
            End Get
            Set(value As Models.Workflow)
                m_Info = value
            End Set
        End Property

        Private m_MergeList As List(Of Integer)
        Public Property MergeList As List(Of Integer)
            Get
                Return m_MergeList
            End Get
            Set(value As List(Of Integer))
                m_MergeList = value
            End Set
        End Property

        Private m_CodeList As List(Of Models.GenericList)
        Public Property CodeList As List(Of Models.GenericList)
            Get
                Return m_CodeList
            End Get
            Set(value As List(Of Models.GenericList))
                m_CodeList = value
            End Set
        End Property
    End Class

    Public Class RecipeWorkflowList
        Private m_ID As Integer
        Public Property ID As Integer
            Get
                Return m_ID
            End Get
            Set(value As Integer)
                m_ID = value
            End Set
        End Property

        Private m_SequenceNo As Integer
        Public Property SequenceNo As Integer
            Get
                Return m_SequenceNo
            End Get
            Set(value As Integer)
                m_SequenceNo = value
            End Set
        End Property

        Private m_Workflow As String
        Public Property Workflow As String
            Get
                Return m_Workflow
            End Get
            Set(value As String)
                m_Workflow = value
            End Set
        End Property

        Private m_Task As String
        Public Property Task As String
            Get
                Return m_Task
            End Get
            Set(value As String)
                m_Task = value
            End Set
        End Property

        Private m_CodeListe As Integer
        Public Property CodeListe As String
            Get
                Return m_CodeListe
            End Get
            Set(value As String)
                m_CodeListe = value
            End Set
        End Property

        Private m_Recipe As String
        Public Property Recipe As String
            Get
                Return m_Recipe
            End Get
            Set(value As String)
                m_Recipe = value
            End Set
        End Property

        Private m_Attachment As String
        Public Property Attachment As String
            Get
                Return m_Attachment
            End Get
            Set(value As String)
                m_Attachment = value
            End Set
        End Property

        Private m_User As String
        Public Property User As String
            Get
                Return m_User
            End Get
            Set(value As String)
                m_User = value
            End Set
        End Property

        Private m_DateTime As String
        Public Property DateTime As String
            Get
                Return m_DateTime
            End Get
            Set(value As String)
                m_DateTime = value
            End Set
        End Property

        Private m_Duration As Double
        Public Property Duration As Double
            Get
                Return m_Duration
            End Get
            Set(value As Double)
                m_Duration = value
            End Set
        End Property

        Private m_TaskStatus As String
        Public Property TaskStatus As String
            Get
                Return m_TaskStatus
            End Get
            Set(value As String)
                m_TaskStatus = value
            End Set
        End Property

        Private m_CodeWorkflowTask As Integer
        Public Property CodeWorkflowTask As Integer
            Get
                Return m_CodeWorkflowTask
            End Get
            Set(value As Integer)
                m_CodeWorkflowTask = value
            End Set
        End Property
    End Class

    Public Class WorkflowTaskUser

        Private m_ID As Integer
        Public Property ID As Integer
            Get
                Return m_ID
            End Get
            Set(value As Integer)
                m_ID = value
            End Set
        End Property

        Private m_WorkflowCode As Integer
        Public Property WorkflowCode As Integer
            Get
                Return m_WorkflowCode
            End Get
            Set(value As Integer)
                m_WorkflowCode = value
            End Set
        End Property

        Private m_Workflow As String
        Public Property Workflow As String
            Get
                Return m_Workflow
            End Get
            Set(value As String)
                m_Workflow = value
            End Set
        End Property

        Private m_TaskCode As Integer
        Public Property TaskCode As Integer
            Get
                Return m_TaskCode
            End Get
            Set(value As Integer)
                m_TaskCode = value
            End Set
        End Property


        Private m_Task As String
        Public Property Task As String
            Get
                Return m_Task
            End Get
            Set(value As String)
                m_Task = value
            End Set
        End Property

        Private m_UserCode As Integer
        Public Property UserCode As Integer
            Get
                Return m_UserCode
            End Get
            Set(value As Integer)
                m_UserCode = value
            End Set
        End Property

        Private m_User As String
        Public Property User As String
            Get
                Return m_User
            End Get
            Set(value As String)
                m_User = value
            End Set
        End Property

        Private m_Duration As Integer
        Public Property Duration As Integer
            Get
                Return m_Duration
            End Get
            Set(value As Integer)
                m_Duration = value
            End Set
        End Property
    End Class

    Public Class RecipeWorkflowData
        Private m_ID As Integer
        Public Property ID As Integer
            Get
                Return m_ID
            End Get
            Set(value As Integer)
                m_ID = value
            End Set
        End Property

        Private m_CodeListe As Integer
        Public Property CodeListe As Integer
            Get
                Return m_CodeListe
            End Get
            Set(value As Integer)
                m_CodeListe = value
            End Set
        End Property

        Private m_RecipeName As String
        Public Property RecipeName As String
            Get
                Return m_RecipeName
            End Get
            Set(value As String)
                m_RecipeName = value
            End Set
        End Property


        Private m_CodeWorkflowTask As Integer
        Public Property CodeWorkflowTask As Integer
            Get
                Return m_CodeWorkflowTask
            End Get
            Set(value As Integer)
                m_CodeWorkflowTask = value
            End Set
        End Property

        Private m_Attachment As String
        Public Property Attachment As String
            Get
                Return m_Attachment
            End Get
            Set(value As String)
                m_Attachment = value
            End Set
        End Property

        Private m_TaskStatus As String
        Public Property TaskStatus As String
            Get
                Return m_TaskStatus
            End Get
            Set(value As String)
                m_TaskStatus = value
            End Set
        End Property

        Private m_DateTime As String
        Public Property DateTime As String
            Get
                Return m_DateTime
            End Get
            Set(value As String)
                m_DateTime = value
            End Set
        End Property

        Private m_UpdateDate As String
        Public Property UpdateDate As String
            Get
                Return m_UpdateDate
            End Get
            Set(value As String)
                m_UpdateDate = value
            End Set
        End Property

        Private m_IsTemp As Boolean
        Public Property IsTemp As String
            Get
                Return m_IsTemp
            End Get
            Set(value As String)
                m_IsTemp = value
            End Set
        End Property
    End Class

    Public Class WorkflowRecipe

        Private m_CodeListe As Integer
        Public Property CodeListe As Integer
            Get
                Return m_CodeListe
            End Get
            Set(value As Integer)
                m_CodeListe = value

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
    End Class

    Public Class WorkflowAttachment
        Private m_Id As Integer
        Public Property Id As Integer
            Get
                Return m_Id
            End Get
            Set(value As Integer)
                m_Id = value
            End Set
        End Property
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
            End Set
        End Property
        Private m_Resource As String
        Public Property Resource As String
            Get
                Return m_Resource
            End Get
            Set(value As String)
                m_Resource = value
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
        Private m_blnDefault As Boolean
        Public Property IsDefault As Boolean
            Get
                Return m_blnDefault
            End Get
            Set(value As Boolean)
                m_blnDefault = value
            End Set
        End Property
    End Class

    Public Class WorkflowDataRecipe
        Private m_RecipeWorkflowData As List(Of Models.RecipeWorkflowData)
        Public Property RecipeWorkflowData As List(Of Models.RecipeWorkflowData)
            Get
                Return m_RecipeWorkflowData
            End Get
            Set(value As List(Of Models.RecipeWorkflowData))
                m_RecipeWorkflowData = value
            End Set
        End Property

        Private m_WorkflowAttachment As Models.WorkflowAttachment
        Public Property WorkflowAttachment As Models.WorkflowAttachment
            Get
                Return m_WorkflowAttachment
            End Get
            Set(value As Models.WorkflowAttachment)
                m_WorkflowAttachment = value
            End Set
        End Property

        Private m_CustomTempAttachments As String
        Public Property CustomTempAttachments As String
            Get
                Return m_CustomTempAttachments
            End Get
            Set(value As String)
                m_CustomTempAttachments = value
            End Set
        End Property
    End Class


End Namespace

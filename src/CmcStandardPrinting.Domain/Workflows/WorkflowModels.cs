namespace CmcStandardPrinting.Domain.Workflows;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;

public sealed class Workflow
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CodeTaskWorkflow { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string WorkflowName { get; set; } = string.Empty;
    public int CodeTask { get; set; }
    public bool Archive { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class WorkflowData
{
    public Workflow Info { get; set; } = new();
    public List<int>? MergeList { get; set; }
    public List<GenericList>? CodeList { get; set; }
}

public sealed class RecipeWorkflowList
{
    public int ID { get; set; }
    public int SequenceNo { get; set; }
    public string Workflow { get; set; } = string.Empty;
    public string Task { get; set; } = string.Empty;
    public int CodeListe { get; set; }
    public string Recipe { get; set; } = string.Empty;
    public string Attachment { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string DateTime { get; set; } = string.Empty;
    public double Duration { get; set; }
    public string TaskStatus { get; set; } = string.Empty;
    public int CodeWorkflowTask { get; set; }
}

public sealed class WorkflowTaskUser
{
    public int ID { get; set; }
    public int WorkflowCode { get; set; }
    public string Workflow { get; set; } = string.Empty;
    public int TaskCode { get; set; }
    public string Task { get; set; } = string.Empty;
    public int UserCode { get; set; }
    public string User { get; set; } = string.Empty;
    public int Duration { get; set; }
}

public sealed class RecipeWorkflowData
{
    public int ID { get; set; }
    public int CodeListe { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public int CodeWorkflowTask { get; set; }
    public string Attachment { get; set; } = string.Empty;
    public string TaskStatus { get; set; } = string.Empty;
    public string DateTime { get; set; } = string.Empty;
    public string UpdateDate { get; set; } = string.Empty;
    public bool IsTemp { get; set; }
}

public sealed class WorkflowRecipe
{
    public int CodeListe { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class WorkflowAttachment
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public sealed class WorkflowDataRecipe
{
    public List<RecipeWorkflowData> RecipeWorkflowData { get; set; } = new();
    public WorkflowAttachment? WorkflowAttachment { get; set; }
    public string CustomTempAttachments { get; set; } = string.Empty;
}

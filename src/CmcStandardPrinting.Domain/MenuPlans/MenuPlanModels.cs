namespace CmcStandardPrinting.Domain.MenuPlans;

using System.Collections.Generic;

public sealed class MenuPlan
{
    public int CodeMenuPlan { get; set; }
    public int CopiedFromMpCode { get; set; }
    public int CodeRestaurantFrom { get; set; }
    public int CodeRestaurantTo { get; set; }
    public bool CopyRestaurant { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CodeRestaurant { get; set; }
    public int CodeCategory { get; set; }
    public int CodeSeason { get; set; }
    public int CodeService { get; set; }
    public bool CyclePlan { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Recurrence { get; set; }
    public int CodeSetPrice { get; set; }
    public int CodeUser { get; set; }
    public int CodeTrans { get; set; }
    public List<MasterplanMapping> Source { get; set; } = new();
}

public sealed class MasterplanMapping
{
    public int CodeMasterPlan { get; set; }
    public int CodeMasterPlanSource { get; set; }
}

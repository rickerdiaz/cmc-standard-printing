namespace CmcStandardPrinting.Domain.RssFeeds;

using System.Collections.Generic;

public sealed class RssFeedStats
{
    public string StatsOverview { get; set; } = string.Empty;
    public List<StatsPerYear>? StatsPerYear { get; set; }
    public List<StatsPerMonth>? StatsPerMonth { get; set; }
    public List<StatsPerDay>? StatsPerDay { get; set; }
    public List<StatsPerHours>? StatsPerHour { get; set; }
}

public sealed class StatsPerYear
{
    public string DetailsPerYear { get; set; } = string.Empty;
}

public sealed class StatsPerMonth
{
    public string DetailsPerMonth { get; set; } = string.Empty;
}

public sealed class StatsPerDay
{
    public string DetailsPerDay { get; set; } = string.Empty;
}

public sealed class StatsPerHours
{
    public string DetailsPerHours { get; set; } = string.Empty;
}

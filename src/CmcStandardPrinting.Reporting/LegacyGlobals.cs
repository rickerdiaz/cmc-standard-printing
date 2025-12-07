namespace EgsReport;

/// <summary>
/// Captures legacy shared state used by the original reporting layer so the new
/// implementation can retain the same configuration surface.
/// </summary>
public static class clsGlobal
{
    public static string G_strPhotoPath { get; set; } = string.Empty;
    public static string G_strLogoPath { get; set; } = string.Empty;
    public static string G_strLogoPath2 { get; set; } = string.Empty;
    public static bool G_IsCalcmenuOnline { get; set; }
}

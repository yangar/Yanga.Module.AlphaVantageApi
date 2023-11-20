namespace Yanga.Module.AlphaVantageApi;
public static class OutputSize
{
    public enum OutputSizeEnum
    {
        Compact,
        Full
    }
    /// <summary>
    ///     Open Ai chat
    /// </summary>
    public static string Compact => "compact";
    /// <summary>
    ///     Google Vertex Ai Chat
    /// </summary>
    public static string Full => "full";
}
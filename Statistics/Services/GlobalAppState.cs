namespace Statistics.Services;

/// <summary>
///     The global application state object that holds objects which are used throughout the application.
///     ...which is just one...for now?
/// </summary>
public class GlobalAppState
{
    /// <summary>
    ///     The main service of the tool.
    ///     It holds the SUS data in a structured format, performs preprocessing, performs all the inferential
    ///     statistics on the data and provides the data for the other services.
    ///     This object holds the SystemUsabilityScale objects, each representing a single system/variable.
    /// </summary>
    public SusStatisticsCalculator? Calculator { get; set; }

    /// <summary>
    ///     Sets the application into debug mode, which at times prints additional information to the console.
    ///     At time of open sourcing the tool, its probably no longer implemented anywhere anyway.
    /// </summary>
    public static bool DebugMode { get; } = false;
}
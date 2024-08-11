namespace Statistics.Pages;

/// <summary>
///     The code-behind for the TestResults page. This page displays the results of the statistical test.
/// </summary>
public partial class TestResults
{
    /// <summary>
    ///     List of all active SUS score lists. Each list contains the SUS scores of a sample/system/variable.
    /// </summary>
    private List<List<float>> _activeSUSScoreLists = new();

    /// <summary>
    ///     List of all active system names. Ordered in the same way as the _activeSUSScoreLists.
    /// </summary>
    private List<string> _systemNames = new();

    /// <summary>
    ///     Initializes the component. Loads the active SUS score lists from the global app state.
    /// </summary>
    protected override void OnInitialized()
    {
        // Check if the Calculator exists
        if (GlobalAppState.Calculator == null) return;

        // Store all scores for each active study into a list
        _activeSUSScoreLists = GlobalAppState.Calculator.ActiveStudies
            .Select(study => study.SUSScores)
            .ToList();

        // Store all names of active study into a list
        _systemNames = GlobalAppState.Calculator.ActiveStudies
            .Select(study => study.SystemName)
            .ToList();
    }
}
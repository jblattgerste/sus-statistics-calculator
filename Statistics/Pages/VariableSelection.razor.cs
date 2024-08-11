using Statistics.Services;

namespace Statistics.Pages;

/// <summary>
///     The variable selection page of the SUS statistics tool. This page allows the user to select the studies they want
///     to compare. They are shown a table with a checkbox next to each study for selection and a box plot of the SUS
///     scores for first insights. The user has to select at least two studies to proceed to the study design page.
/// </summary>
public partial class VariableSelection
{
    /// <summary>
    ///     The list of SUS scores for each study and the corresponding system names.
    /// </summary>
    private readonly List<List<float>> _susScoreLists = new();

    private readonly List<string> _systemNames = new();

    /// <summary>
    ///     The global calculator instance will be assigned in the OnInitialized method to this.
    /// </summary>
    private SusStatisticsCalculator? _calculator;

    /// <summary>
    ///     Indicates if at least two studies are selected.
    /// </summary>
    private bool AreAtLeastTwoStudiesSelected => _calculator is { ActiveStudies.Count: > 1 };

    /// <summary>
    ///     Initializes the component by setting the calculator and extracting the SUS scores and system names from the
    ///     studies. They are then bind to the PlotlyBoxPlot and table in the razor component.
    /// </summary>
    protected override void OnInitialized()
    {
        // Fetch the calculator from the global app state.
        _calculator = GlobalAppState.Calculator;

        // Extract the SUS scores and system names from the studies.
        if (_calculator?.Studies == null)
            return;

        foreach (SUSStudy study in _calculator?.Studies!)
        {
            _susScoreLists.Add(study.SUSScores);
            _systemNames.Add(study.SystemName);
        }

        _calculator.ActiveStudies = new List<SUSStudy>(_calculator.Studies);
    }

    /// <summary>
    ///     Toggles the selection of a study. If the study is selected, it is added to the list of active studies. If it is
    ///     deselected, it is removed from the list of active studies.
    /// </summary>
    private void ToggleStudySelection(SUSStudy study, bool isSelected)
    {
        if (!isSelected)
            _calculator?.ActiveStudies.Remove(study);
        else
            _calculator?.ActiveStudies.Add(study);

        // Sort ActiveStudies to maintain the order based on Studies
        if (_calculator != null)
            _calculator.ActiveStudies = _calculator.ActiveStudies.OrderBy(s => _calculator.Studies.IndexOf(s)).ToList();

        // Rerender the component to update the proceed button.
        StateHasChanged();
    }

    /// <summary>
    ///     Proceed to the study design page if at least two studies are selected.
    /// </summary>
    private void Proceed()
    {
        if (AreAtLeastTwoStudiesSelected)
            NavigationManager.NavigateTo("./StudyDesignSelection");
    }
}
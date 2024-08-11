using Statistics.Components.Helpers;
using Statistics.Services;

namespace Statistics.Pages;

/// <summary>
///     The code-behind for the StatisticalMethodSelection page.
/// </summary>
public partial class StatisticalMethodSelection
{
    /// <summary>
    ///     Modal window to inform the user that the friedman test is currently not supported.
    /// </summary>
    private ModalWindow? _friedmanNotSupportedModal;

    /// <summary>
    ///     Modal window to inform the user that the kruskal-wallis test is currently not supported.
    /// </summary>
    private ModalWindow? _kruskalWallisNotSupportedModal;

    /// <summary>
    ///     Modal window to inform the user that the repeated measures ANOVA is currently not supported.
    /// </summary>
    private ModalWindow? _repeatedANOVANotSupportedModal;

    /// <summary>
    ///     Proceed with a parametric test (e.g. t-test, ANOVA) - only possible if we have 2 or less studies or independent
    ///     samples
    /// </summary>
    private void ProceedWithParametricTest()
    {
        if (GlobalAppState.Calculator != null
            && GlobalAppState.Calculator.ActiveStudies.Count > 2
            && GlobalAppState.Calculator.SampleDependence == SampleDependence.DependentSample)
        {
            // If we want to use parametric tests with more than 2 studies for repeated measures, we would need to implement
            // a repeated measures ANOVA (with assumptions and corrections). We dont have that at the moment so in this case,
            // prompt the user that this is not supported
            _ = _repeatedANOVANotSupportedModal?.Open();
        }
        else // Independent samples t-test, paired t-test, ANOVA
        {
            // Tell the SUSStatisticsCalculator that the user wants to use a parametric test and proceed to the next step
            if (GlobalAppState.Calculator != null) GlobalAppState.Calculator.Parametric = Parametric.Parametric;
            NavigationManager.NavigateTo("./TestResults");
        }
    }

    /// <summary>
    ///     Proceed with non-parametric test (e.g. Wilcoxon, Friedman, Kruskal-Wallis, Mann-Whitney) depending on the study
    ///     design an d number of studies
    /// </summary>
    private void ProceedWithNonParametricTest()
    {
        if (GlobalAppState.Calculator != null
            && GlobalAppState.Calculator.ActiveStudies.Count > 2
            && GlobalAppState.Calculator.SampleDependence ==
            SampleDependence.IndependentSample)
        {
            // Kruskal-Wallis is not supported as well.
            _ = _kruskalWallisNotSupportedModal?.Open();
        }
        else if (GlobalAppState.Calculator != null
                 && GlobalAppState.Calculator.ActiveStudies.Count > 2
                 && GlobalAppState.Calculator.SampleDependence == SampleDependence.DependentSample)
        {
            // Friedman is not supported as well.
            _ = _friedmanNotSupportedModal?.Open();
        }
        else // Mann-Whitney, Wilcoxon
        {
            // Tell the SUSStatisticsCalculator that the user wants to use a non-parametric test and proceed to the next step
            GlobalAppState.Calculator!.Parametric = Parametric.NonParametric;
            NavigationManager.NavigateTo("./TestResults");
        }
    }
}
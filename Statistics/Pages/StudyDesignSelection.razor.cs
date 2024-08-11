using Statistics.Components.Helpers;
using Statistics.Services;

namespace Statistics.Pages;

/// <summary>
///     The code-behind class for the StudyDesignSelection page.
/// </summary>
public partial class StudyDesignSelection
{
    /// <summary>
    ///     Modal window to inform the user that different sample sizes were detected and that a dependent sample comparison
    ///     requires the same number of data points for each system/variable.
    /// </summary>
    private ModalWindow? _differentSampleSizesModal;

    /// <summary>
    ///     Proceed with dependent samples (e.g. within subject comparison)
    /// </summary>
    private void ProceedWithDependentSamples()
    {
        //Iterate through GlobalAppState.Calculator.ActiveStudies and check if they are all the same sample size
        if (GlobalAppState.Calculator != null
            && GlobalAppState.Calculator.ActiveStudies.Any(study =>
                study.SUSScores.Count != GlobalAppState.Calculator.ActiveStudies[0].SUSScores.Count))
        {
            //If they are not, they cannot possibly be a dependent sample and there might be a mistake in data or user intend
            _ = _differentSampleSizesModal?.Open();
        }
        else
        {
            // Tell the SUSStatisticsCalculator that we are working with dependent samples and proceed to the next step
            if (GlobalAppState.Calculator != null)
                GlobalAppState.Calculator.SampleDependence = SampleDependence.DependentSample;
            NavigationManager.NavigateTo("./StatisticalMethodSelection");
        }
    }

    /// <summary>
    ///     Proceed with independent samples (e.g. between subject comparison)
    /// </summary>
    private void ProceedWithIndependentSamples()
    {
        // Tell the SUSStatisticsCalculator that we are working with independent samples and proceed to the next step
        if (GlobalAppState.Calculator != null)
            GlobalAppState.Calculator.SampleDependence = SampleDependence.IndependentSample;
        NavigationManager.NavigateTo("./StatisticalMethodSelection");
    }
}
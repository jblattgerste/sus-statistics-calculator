using System.Globalization;
using Accord.Statistics;
using Accord.Statistics.Testing;
using Microsoft.AspNetCore.Components;

namespace Statistics.Components.StatisticResults;

/// <summary>
///     The code-behind for the Mann-Whitney U test result component.
/// </summary>
public partial class MannWhitneyUTest
{
    /// <summary>
    ///     The data to be used in the Mann-Whitney U test.
    /// </summary>
    [Parameter]
    public required List<List<float>>? Data { get; init; }

    /// <summary>
    ///     The names of the two systems being compared.
    /// </summary>
    [Parameter]
    public required List<string>? SystemNames { get; init; }

    /// <summary>
    ///     The result of the Mann-Whitney U test.
    /// </summary>
    private MannWhitneyWilcoxonTest? Result { get; set; }

    /// <summary>
    ///     The APA style result string of the Mann-Whitney U test.
    /// </summary>
    private string? ApaStyleResult { get; set; }

    /// <summary>
    ///     On initialization, perform the Mann-Whitney U test.
    /// </summary>
    protected override void OnInitialized()
    {
        if (Data is { Count: 2 }) PerformMannWhitneyUTest();
    }

    /// <summary>
    ///     Perform the Mann-Whitney U test using the provided parameters. The result is stored in the <see cref="Result" />
    ///     property.
    /// </summary>
    private void PerformMannWhitneyUTest()
    {
        // Convert the data to double arrays
        double[]? sample1 = Data?[0].Select(x => (double)x).ToArray();
        double[]? sample2 = Data?[1].Select(x => (double)x).ToArray();

        // Perform the Mann-Whitney U test if the data is available
        if (sample1 != null && sample2 != null) Result = new MannWhitneyWilcoxonTest(sample1, sample2);

        // Generate the APA style result string
        GenerateApaStyleResult();
    }

    /// <summary>
    ///     Generate the APA style result string for the Mann-Whitney U test. The result is stored in the
    ///     <see cref="ApaStyleResult" /> property.
    /// </summary>
    private void GenerateApaStyleResult()
    {
        // Check if the data and result are available
        if (Data == null || Result == null) return;

        // Determine the significance description based on the test result and construct the APA style result string
        string significanceDescription = Result is { Significant: true } ? "a significant" : "no significant";
        ApaStyleResult =
            $"A Mann-Whitney U test was conducted to determine whether there were significant differences between SUS scores of the two systems. " +
            $"There was {significanceDescription} difference in scores for {SystemNames?[0]} " +
            $"(Mdn = {Data[0].Select(x => (double)x).ToArray().Median():F2}) and {SystemNames?[1]} " +
            $"(Mdn = {Data[1].Select(x => (double)x).ToArray().Median():F2}); U = {Result.Statistic:F2}, " +
            $"p = {Result.PValue.ToString("F4", CultureInfo.InvariantCulture)}.";
    }
}
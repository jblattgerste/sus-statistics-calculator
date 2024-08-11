using System.Globalization;
using Accord.Statistics;
using Accord.Statistics.Testing;
using Microsoft.AspNetCore.Components;

namespace Statistics.Components.StatisticResults;

/// <summary>
///     The code-behind for the independent samples t-test result component.
/// </summary>
public partial class IndependentSamplesTTest
{
    /// <summary>
    ///     The data to be used in the independent samples t-test.
    /// </summary>
    [Parameter]
    public required List<List<float>>? Data { get; init; }

    /// <summary>
    ///     The names of the two systems being compared.
    /// </summary>
    [Parameter]
    public required List<string>? SystemNames { get; init; }

    /// <summary>
    ///     The result of the independent samples t-test as a struct.
    /// </summary>
    private TwoSampleTTest? Result { get; set; }

    /// <summary>
    ///     The effect size of the independent samples t-test.
    /// </summary>
    private double EffectSize { get; set; }

    /// <summary>
    ///     The APA style result string of the independent samples t-test.
    /// </summary>
    private string? ApaStyleResult { get; set; }

    /// <summary>
    ///     Initializes the component and performs the independent samples t-test using the provided parameters.
    /// </summary>
    protected override void OnInitialized()
    {
        if (Data is { Count: 2 }) PerformTTest();
    }

    /// <summary>
    ///     Performs the independent samples t-test using the provided data.
    /// </summary>
    private void PerformTTest()
    {
        // Convert data to double arrays
        double[]? sample1 = Data?[0].Select(x => (double)x).ToArray();
        double[]? sample2 = Data?[1].Select(x => (double)x).ToArray();

        // Perform independent samples t-test
        Result = new TwoSampleTTest(sample1, sample2);

        // Calculate Cohen's d effect size
        if (sample1 != null && sample2 != null)
        {
            double pooledStandardDeviation = Math.Sqrt(
                ((sample1.Length - 1) * sample1.Variance() + (sample2.Length - 1) * sample2.Variance()) /
                (sample1.Length + sample2.Length - 2)
            );
            EffectSize = Math.Abs(Result.ObservedDifference) / pooledStandardDeviation;
        }

        // Generate APA style result string
        GenerateApaStyleResult();
    }

    /// <summary>
    ///     Generates the APA style result string based on the independent samples t-test result.
    /// </summary>
    private void GenerateApaStyleResult()
    {
        // Check if the results are significant and generate the APA style result string
        string significanceDescription = Result is { Significant: true } ? "a significant" : "no significant";
        string effectSizeDescription = DescribeEffectSize(EffectSize);
        if (Data != null && Result != null)
            ApaStyleResult =
                $"An independent-samples t-test was conducted to compare the means between the two SUS study scores. " +
                $"There was {significanceDescription} difference in scores for {SystemNames?[0]} (M = {Data[0].Average():F2}, SD = {Data[0].Select(x => (double)x).ToArray().StandardDeviation():F2}) " +
                $"and {SystemNames?[1]} (M = {Data[1].Average():F2}, SD = {Data[1].Select(x => (double)x).ToArray().StandardDeviation():F2}); " +
                $"t({Result.DegreesOfFreedom:F2}) = {Result.Statistic:F2}, p = {Result.PValue.ToString("F4", CultureInfo.InvariantCulture)}. " +
                $"The magnitude of the differences in the means (mean difference = {Result.ObservedDifference:F2}, " +
                $"95% CI: {Result.Confidence.Min:F2} to {Result.Confidence.Max:F2}) " +
                $"is considered {effectSizeDescription} (Cohen's d = {EffectSize:F2}).";
    }

    /// <summary>
    ///     Describes the effect size based on the Cohen's d.
    /// </summary>
    private string DescribeEffectSize(double d)
    {
        switch (d)
        {
            case < 0.2:
                return "very small";
            case < 0.5:
                return "small";
            case < 0.8:
                return "medium";
            default:
                return "large";
        }
    }
}
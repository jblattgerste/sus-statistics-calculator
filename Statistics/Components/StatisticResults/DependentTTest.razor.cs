using System.Globalization;
using Accord.Statistics;
using Accord.Statistics.Testing;
using Microsoft.AspNetCore.Components;

namespace Statistics.Components.StatisticResults;

/// <summary>
///     The code-behind partial class for the DependentTTest component.
/// </summary>
public partial class DependentTTest
{
    /// <summary>
    ///     Data represents the two samples to be compared.
    /// </summary>
    [Parameter]
    public required List<List<float>>? Data { get; init; }

    /// <summary>
    ///     SystemNames represents the names of the two systems being compared.
    /// </summary>
    [Parameter]
    public required List<string>? SystemNames { get; init; }

    /// <summary>
    ///     Result represents the paired-samples t-test result struct.
    /// </summary>
    private PairedTTest? Result { get; set; }

    /// <summary>
    ///     EffectSize represents the Cohen's d effect size for the paired-samples t-test.
    /// </summary>
    private double EffectSize { get; set; }

    /// <summary>
    ///     ApaStyleResult is a string that contains the APA style result of the paired-samples t-test.
    /// </summary>
    private string? ApaStyleResult { get; set; }

    /// <summary>
    ///     On initialization of the component, perform the paired-samples t-test on the parameters provided.
    /// </summary>
    protected override void OnInitialized()
    {
        if (Data is { Count: 2 }) PerformPairedTTest();
    }

    /// <summary>
    ///     Perform the paired-samples t-test on the provided data.
    /// </summary>
    private void PerformPairedTTest()
    {
        double[]? sample1 = Data?[0].Select(x => (double)x).ToArray();
        double[]? sample2 = Data?[1].Select(x => (double)x).ToArray();

        if (sample1 == null || sample2 == null) return;

        Result = new PairedTTest(sample1, sample2);

        // Calculate Cohen's d effect size for paired samples
        double meanDifference = Result.ObservedDifference;
        double sdDifference = Result.StandardError * Math.Sqrt(sample1.Length);
        EffectSize = Math.Abs(meanDifference) / sdDifference;

        GenerateApaStyleResult();
    }

    /// <summary>
    ///     Generate the APA style result string for the paired-samples t-test.
    /// </summary>
    private void GenerateApaStyleResult()
    {
        if (Result == null || Data?[0] == null || Data?[1] == null) return;

        string significanceDescription = Result.Significant ? "a significant" : "no significant";
        string effectSizeDescription = DescribeEffectSize(EffectSize);

        ApaStyleResult =
            $"A paired-samples t-test was conducted to compare {SystemNames?[0]} and {SystemNames?[1]} SUS scores. " +
            $"There was {significanceDescription} difference in SUS scores for {SystemNames?[0]} (M = {Data?[0].Average():F2}, SD = {Data?[0].Select(x => (double)x).ToArray().StandardDeviation():F2}) " +
            $"and {SystemNames?[1]} (M = {Data?[1].Average():F2}, SD = {Data?[1].Select(x => (double)x).ToArray().StandardDeviation():F2}), " +
            $"t({Result.SampleSize - 1}) = {Result.Statistic:F2}, p = {Result.PValue.ToString("F4", CultureInfo.InvariantCulture)}. " +
            $"The mean difference was {Result.ObservedDifference:F2} " +
            $"(95% CI: {Result.Confidence.Min:F2} to {Result.Confidence.Max:F2}). " +
            $"This would be considered a {effectSizeDescription} effect size (Cohen's d = {EffectSize:F2}).";
    }

    /// <summary>
    ///     Describes the effect size based on Cohen's d value with the common thresholds:
    ///     https://en.wikipedia.org/wiki/Effect_size#:~:text=Cohen's%20d%20is%20defined%20as,similarly%20for%20the%20other%20group.
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
using System.Globalization;
using Accord.Statistics;
using Accord.Statistics.Testing;
using Microsoft.AspNetCore.Components;

namespace Statistics.Components.StatisticResults;

/// <summary>
///     The code-behind for the Wilcoxon signed-rank test component.
/// </summary>
public partial class WilcoxonSignedRankTest
{
    /// <summary>
    ///     The data to be used in the Wilcoxon signed-rank test.
    /// </summary>
    [Parameter]
    public required List<List<float>>? Data { get; init; }

    /// <summary>
    ///     The names of the systems being compared.
    /// </summary>
    [Parameter]
    public required List<string>? SystemNames { get; init; }

    /// <summary>
    ///     The result of the Wilcoxon signed-rank test form Accord.NET
    /// </summary>
    private TwoSampleWilcoxonSignedRankTest? Result { get; set; }

    /// <summary>
    ///     The APA style result string for the Wilcoxon signed-rank test.
    /// </summary>
    private string? ApaStyleResult { get; set; }

    /// <summary>
    ///     On initialization, perform the Wilcoxon signed-rank test.
    /// </summary>
    protected override void OnInitialized()
    {
        if (Data is { Count: 2 }) PerformWilcoxonSignedRankTest();
    }

    /// <summary>
    ///     Perform the Wilcoxon signed-rank test. The result is stored in the <see cref="Result" /> property.
    /// </summary>
    private void PerformWilcoxonSignedRankTest()
    {
        double[]? sample1 = Data?[0].Select(x => (double)x).ToArray();
        double[]? sample2 = Data?[1].Select(x => (double)x).ToArray();

        if (sample1 == null || sample2 == null) return;
        Result = new TwoSampleWilcoxonSignedRankTest(sample1, sample2);
        GenerateApaStyleResult();
    }

    /// <summary>
    ///     Generate the APA style result string for the Wilcoxon signed-rank test. The result is stored in the
    ///     <see cref="ApaStyleResult" /> property.
    /// </summary>
    private void GenerateApaStyleResult()
    {
        // Check if the required data is available
        if (Result == null || Data == null || SystemNames == null) return;

        // Determine the significance description
        string significanceDescription = Result.Significant ? "a significant" : "no significant";

        // Calculate medians and interquartile ranges for both samples
        double[] sample1 = Data[0].Select(x => (double)x).ToArray();
        double[] sample2 = Data[1].Select(x => (double)x).ToArray();
        double median1 = sample1.Median();
        double median2 = sample2.Median();
        double q11 = sample1.Quantile(0.25);
        double q31 = sample1.Quantile(0.75);
        double iqr1 = q31 - q11;
        double q12 = sample2.Quantile(0.25);
        double q32 = sample2.Quantile(0.75);
        double iqr2 = q32 - q12;

        // Generate the APA style result string for the Wilcoxon signed-rank test that is displayed to the user
        ApaStyleResult =
            $"A Wilcoxon signed-rank test was conducted to determine whether there was a significant difference in SUS scores. " +
            $"There was {significanceDescription} difference in scores for {SystemNames[0]} " +
            $"(Mdn = {median1:F2}, IQR = {iqr1:F2}) and {SystemNames[1]} " +
            $"(Mdn = {median2:F2}, IQR = {iqr2:F2}); Z = {Result.Statistic:F2}, " +
            $"p = {Result.PValue.ToString("F4", CultureInfo.InvariantCulture)}. " +
            $"The test {(Result.Significant ? "rejected" : "failed to reject")} the null hypothesis. " +
            $"{(Result.HasTies ? "The test detected ties in the data. " : "")}" +
            $"{(Result.HasZeros ? "The test detected pairs with zero difference in the data. " : "")}";
    }
}
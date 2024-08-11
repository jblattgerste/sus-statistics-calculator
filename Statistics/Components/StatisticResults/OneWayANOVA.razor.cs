using System.Globalization;
using System.Text;
using Accord.Statistics;
using Accord.Statistics.Testing;
using Microsoft.AspNetCore.Components;

namespace Statistics.Components.StatisticResults;

/// <summary>
///     The code-behind for the OneWayANOVA component.
/// </summary>
public partial class OneWayANOVA
{
    /// <summary>
    ///     The data for the one-way ANOVA. The data should be a list of lists, where each list represents a sample of data
    ///     (List<float>).
    /// </summary>
    [Parameter]
    public required List<List<float>>? Data { get; init; }

    /// <summary>
    ///     The system names for the data.
    /// </summary>
    [Parameter]
    public required List<string>? SystemNames { get; init; }

    /// <summary>
    ///     The results of the one-way ANOVA.
    /// </summary>
    private OneWayAnova? Result { get; set; }

    /// <summary>
    ///     A boolean indicating whether the ANOVA is significant.
    /// </summary>
    private bool IsANOVASignificant => Result?.FTest.Significant ?? false;

    /// <summary>
    ///     A list of the post-hoc results of the ANOVA.
    /// </summary>
    private List<PostHocResult>? PostHocResults { get; set; }

    /// <summary>
    ///     The effect size (η²) for the ANOVA.
    /// </summary>
    private double EffectSize { get; set; }

    /// <summary>
    ///     A tuple of strings for the APA style result. The first string is the descriptive statistics, the second string is
    ///     the ANOVA result and the third string is the post-hoc tests.
    /// </summary>
    private (string, string, string)? ApaStyleResult { get; set; }

    /// <summary>
    ///     On initialized, perform the one-way ANOVA with the provided parameters.
    /// </summary>
    protected override void OnInitialized()
    {
        if (Data != null && Data.Count >= 3) PerformOneWayANOVA();
    }

    /// <summary>
    ///     Performs a one-way ANOVA on the provided data. Includes post-hoc tests with Bonferroni corrected t-tests.
    /// </summary>
    private void PerformOneWayANOVA()
    {
        // Convert data to jagged array
        double[][] samples = Data!.Select(group => group.Select(x => (double)x).ToArray()).ToArray();

        // Perform One-Way ANOVA
        Result = new OneWayAnova(samples);

        // Calculate effect size (η²)
        EffectSize = CalculateEtaSquared(samples);

        // Perform post-hoc t-tests with Bonferroni correction
        PostHocResults = PerformPostHocTests(samples);

        // Generate APA style result
        GenerateApaStyleResult();
    }

    /// <summary>
    ///     Calculates the effect size (η²) for the ANOVA. This is also known as eta squared (the common language effect size).
    /// </summary>
    private double CalculateEtaSquared(double[][] samples)
    {
        // Calculate the total sum of squares (SS) and between-group sum of squares (SS) for the ANOVA. Return the effect size (betweenSs / totalSs).
        double totalSs = samples.SelectMany(x => x).Sum(x => Math.Pow(x - samples.SelectMany(y => y).Average(), 2));
        double betweenSs = samples.Sum(group =>
            group.Length * Math.Pow(group.Average() - samples.SelectMany(y => y).Average(), 2));
        return betweenSs / totalSs;
    }

    /// <summary>
    ///     Performs post-hoc tests using Bonferroni correction.
    /// </summary>
    private List<PostHocResult> PerformPostHocTests(double[][] samples)
    {
        // Create a list to store the results of the post-hoc tests in a structured way
        List<PostHocResult> results = new();

        // Calculate the number of comparisons and calculate the Bonferroni-corrected alpha level
        int numComparisons = samples.Length * (samples.Length - 1) / 2;
        double bonferroniAlpha = 0.05 / numComparisons;

        // Perform all pairwise comparisons on the provided samples
        for (int i = 0; i < samples.Length; i++)
        for (int j = i + 1; j < samples.Length; j++)
        {
            // Perform a two-sample t-test
            TwoSampleTTest tTest = new(samples[i], samples[j]);

            // Store the results in the list of struct results
            results.Add(new PostHocResult
            {
                Group1 = i,
                Group2 = j,
                PValue = tTest.PValue,
                Significant = tTest.PValue < bonferroniAlpha,
                MeanDifference = tTest.ObservedDifference,
                Statistic = tTest.Statistic,
                DegreesOfFreedom = tTest.DegreesOfFreedom,
                ConfidenceMin = tTest.Confidence.Min,
                ConfidenceMax = tTest.Confidence.Max
            });
        }

        return results;
    }

    /// <summary>
    ///     Generates a stringbuilder to generate APA style result that can be displayed in the UI. The result is stored int
    ///     the <see cref="ApaStyleResult" /> property. This time it is a tuple out of strings for the descriptive results,
    ///     ANOVA and then post-hoc tests.
    /// </summary>
    private void GenerateApaStyleResult()
    {
        // Check if all required data is available
        if (Result == null || PostHocResults == null || Data == null || SystemNames == null) return;

        // Descriptive statistics
        StringBuilder firstString = new();
        for (int i = 0; i < Data.Count; i++)
        {
            double mean = Data[i].Average();
            double sd = Data[i].Select(x => (double)x).ToArray().StandardDeviation();
            firstString.AppendLine($"The SUS study score (mean) of {SystemNames[i]} is {mean:F2} (SD = {sd:F2}).");
        }

        // ANOVA results
        StringBuilder secondString = new();
        string significanceDescription = Result.FTest.Significant ? "a significant" : "no significant";
        secondString.AppendLine(
            $"A one-way ANOVA was conducted to compare the effect of the system/variable on SUS scores for the {SystemNames.Count} conditions.");
        secondString.AppendLine(
            $"There is {significanceDescription} effect of the system/variable on SUS scores at the p < .05 level for the {SystemNames.Count} conditions ");
        secondString.AppendLine(
            $"[F({Result.FTest.DegreesOfFreedom1}, {Result.FTest.DegreesOfFreedom2}) = {Result.FTest.Statistic:F2}, p = {Result.FTest.PValue.ToString("F4", CultureInfo.InvariantCulture)}].");
        secondString.AppendLine();
        // Effect size (η²)
        string effectSizeDescription = DescribeEffectSize(EffectSize);
        secondString.AppendLine(
            $"The effect size (η²) is {EffectSize:F2}, which is generally considered a {effectSizeDescription} effect.");
        secondString.AppendLine();

        // Post hoc results
        StringBuilder thirdString = new();
        thirdString.AppendLine("Post-hoc pairwise comparisons using Bonferroni-corrected t-tests indicated that:");
        foreach (PostHocResult result in PostHocResults)
        {
            string comparisonSignificance = result.Significant ? "significant" : "not significant";
            thirdString.AppendLine(
                $"The mean SUS score of {SystemNames[result.Group1]} (M = {Data[result.Group1].Average():F2}, SD = {Data[result.Group1].Select(x => (double)x).ToArray().StandardDeviation():F2}) is {comparisonSignificance}ly different from the mean SUS Score of {SystemNames[result.Group2]} (M = {Data[result.Group2].Average():F2}, SD = {Data[result.Group2].Select(x => (double)x).ToArray().StandardDeviation():F2}), ");
            thirdString.AppendLine(
                $"t({result.DegreesOfFreedom:F2}) = {result.Statistic:F2}, p = {result.PValue.ToString("F4", CultureInfo.InvariantCulture)}, 95% CI [{result.ConfidenceMin:F2}, {result.ConfidenceMax:F2}].");
            thirdString.AppendLine(
                $"The mean difference in SUS scores between these two groups is {Math.Abs(result.MeanDifference):F2}.");
        }

        // Set the tuple of strings to the ApaStyleResult property
        ApaStyleResult = (firstString.ToString(), secondString.ToString(), thirdString.ToString());
    }

    /// <summary>
    ///     The effect size description/interpretation for the ANOVA based on eta squared.
    /// </summary>
    private string DescribeEffectSize(double etaSquared)
    {
        switch (etaSquared)
        {
            case < 0.06:
                return "small";
            case < 0.14:
                return "medium";
            default:
                return "large";
        }
    }

    /// <summary>
    ///     The result of a post-hoc test as a struct.
    /// </summary>
    private struct PostHocResult
    {
        public int Group1 { get; set; }
        public int Group2 { get; set; }
        public double PValue { get; set; }
        public bool Significant { get; set; }
        public double MeanDifference { get; set; }
        public double Statistic { get; set; }
        public double DegreesOfFreedom { get; set; }
        public double ConfidenceMin { get; set; }
        public double ConfidenceMax { get; set; }
    }
}
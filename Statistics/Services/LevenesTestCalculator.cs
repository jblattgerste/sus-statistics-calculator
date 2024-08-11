using Accord.Statistics.Testing;

namespace Statistics.Services;

/// <summary>
///     The LevenesTestCalculator class provides a method to perform Levene's test for equality of variances on a list of
///     lists of float values and returns the result as a struct with the test statistic, p-value, and an APA-style result
///     description.
///     Levene's test is a statistical test used to assess the equality of variances in different samples.
///     In statistical terms, it tests the null hypothesis that the variances are equal against the alternative
///     hypothesis that at least one variance is different.
/// </summary>
public static class LevenesTestCalculator
{
    /// <summary>
    ///     Performs Levene's test for equality of variances on a list of lists of float values and returns the result as a
    ///     struct with the test statistic, p-value, and an APA-style result description.
    /// </summary>
    /// <param name="data">The list of lists of float values to perform the test on (the data for multiple groups/variables).</param>
    /// <returns>The result of the Levene's test as a LevenesTestResult struct.</returns>
    /// <exception cref="ArgumentException">Thrown when the data is null or empty.</exception>
    public static LevenesTestResult PerformTest(List<List<float>> data)
    {
        // Check if the data is null or empty
        if (data == null || data.Count == 0) throw new ArgumentException("Data cannot be null or empty.");

        // Convert the list of lists of floats to an array of doubles for Accord.NET
        double[][] dataArray = data.Select(d => d.ConvertAll(f => (double)f).ToArray()).ToArray();

        // Perform the Levene's test - use the median version for robustness to skewness
        LeveneTest leveneTest = new(dataArray, true);

        // Interpret the results
        LevenesTestResult result = new()
        {
            F = (float)leveneTest.Statistic,
            PValue = (float)leveneTest.PValue,
            Significance = leveneTest.Significant,
            ApaStyleResultDescription =
                GetApaStyleResultDescription(leveneTest)
        };

        return result;
    }

    /// <summary>
    ///     Generates an APA-style result description for the given Levene's test.
    /// </summary>
    /// <param name="leveneTest">The Accord.NET Levene's test</param>
    /// <returns>The APA-style result description as a string.</returns>
    private static string GetApaStyleResultDescription(LeveneTest leveneTest)
    {
        // Generate an APA-style result description based on the test results
        string varianceConclusion =
            !leveneTest.Significant
                ? "Levene's test did not indicate a significant difference in variances between the provided groups"
                : "Levene's test did indicate a significant difference in variances between the provided groups";
        return
            $"{varianceConclusion}, (F({Math.Round((float)leveneTest.DegreesOfFreedom1, 2)}, {Math.Round((float)leveneTest.DegreesOfFreedom2, 2)}) = {Math.Round(leveneTest.Statistic, 2)}," +
            $" p = {Math.Round(leveneTest.PValue, 2)})";
    }
}

/// <summary>
///     Represents the result of a Levene's test for equality of variances, including the test statistic, p-value, and an
///     APA-style result description.
/// </summary>
public readonly struct LevenesTestResult
{
    /// <summary>
    ///     Represents the F value of a Levene's test for equality of variances.
    /// </summary>
    public float F { get; init; }

    /// <summary>
    ///     Represents the p-value of the Levene's test for equality of variances.
    /// </summary>
    public float PValue { get; init; }

    /// <summary>
    /// Bool indicating whether the test result of Levenes test is significant.
    /// </summary>
    public bool Significance { get; init; }

    /// <summary>
    ///     Returns an APA-style result description of the Levene's test for equality of variances based on the test results.
    /// </summary>
    public string ApaStyleResultDescription { get; init; }
}
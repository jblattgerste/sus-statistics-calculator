using Accord.Statistics.Testing;

namespace Statistics.Services;

/// <summary>
///     The ShapiroWilkTestCalculator class provides a method to perform the Shapiro-Wilk test for normality on a list of
///     float values and returns the result as a struct with the test statistic, p-value, sample size, and a boolean
///     indicating normality.
///     The Shapiro-Wilk test is a statistical test used to assess whether a given sample of data likely came from a
///     normally distributed population.
///     In statistical terms, it tests the null hypothesis that the data is normally distributed against the alternative
///     hypothesis that it is not normally distributed.
/// </summary>
public static class ShapiroWilkTestCalculator
{
    /// <summary>
    ///     Performs the Shapiro-Wilk test for normality on a list of float values and returns the result as a struct
    ///     with the test statistic, p-value, sample size, and a boolean indicating for the normality.
    /// </summary>
    /// <param name="data">The list of float values to perform the test on (The SUS scores of one system/variable).</param>
    /// <returns>The result of the Shapiro-Wilk test as a ShapiroWilkTestResult struct.</returns>
    /// <exception cref="ArgumentException">Thrown when the data is null or empty.</exception>
    public static ShapiroWilkTestResult PerformTest(List<float> data)
    {
        if (data == null || data.Count == 0) throw new ArgumentException("Data cannot be null or empty.");

        // Convert the list of floats to a double array for Accord.NET
        double[] dataArray = data.ConvertAll(d => (double)d).ToArray();

        // Perform the Shapiro-Wilk test
        ShapiroWilkTest shapiroTest = new(dataArray);

        // Interpret the results and return them as a struct with the test statistic, p-value, sample size, normality, and an APA style result description
        ShapiroWilkTestResult result = new()
        {
            W = (float)shapiroTest.Statistic,
            PValue = (float)shapiroTest.PValue,
            SampleSize = dataArray.Length,
            IsNormal = shapiroTest.PValue > 0.05 // Common threshold for normality
        };

        return result;
    }
}

/// <summary>
///     Represents the result of a Shapiro-Wilk test for normality, including normality as a bool for visualization
///     purposes and an APA style result description of the results as a string.
/// </summary>
public readonly struct ShapiroWilkTestResult
{
    /// <summary>
    ///     Represents the W value of a Shapiro-Wilk test for normality.
    /// </summary>
    public float W { get; init; }

    /// <summary>
    ///     Represents the p-value of the Shapiro-Wilk test for normality.
    /// </summary>
    public float PValue { get; init; }

    /// <summary>
    ///     Represents the sample size used in the Shapiro-Wilk test for normality.
    /// </summary>
    public int SampleSize { get; init; }

    /// <summary>
    ///     Indicates whether a distribution is considered normal based on the result of a Shapiro-Wilk test for normality.
    /// </summary>
    public bool IsNormal { get; init; }

    /// <summary>
    ///     Returns an APA style result description of the Shapiro-Wilk test for normality based on the test results.
    /// </summary>
    /// <returns>A string representing the APA style result description.</returns>
    public string ApaStyleResultDescription()
    {
        string normalityConclusion =
            IsNormal
                ? "A Shapiro-Wilk test did not indicate a significant deviation from normality"
                : "A Shapiro-Wilk test did indicate a significant deviation from normality";
        return $"{normalityConclusion}, (W({SampleSize}) = {W:F2}, p = {PValue:F2})";
    }
}
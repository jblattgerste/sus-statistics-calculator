namespace Statistics.Services;

/// <summary>
///     Provides methods for calculating Tukey fences and identifying outliers using the Tukey fences method.
/// </summary>
public static class TukeyFencesCalculator
{
    /// <summary>
    ///     Calculates the Tukey fences for a given set of data directly.
    /// </summary>
    /// <param name="data">The set of data for which to calculate the Tukey fences.</param>
    /// <param name="k">
    ///     The coefficient to multiply the interquartile range (IQR) by to determine the fence distance. Commonly
    ///     used is 1.5f.
    /// </param>
    /// <returns>A tuple containing the lower fence and upper fence.</returns>
    public static (float LowerFence, float UpperFence) CalculateTukeyFences(IEnumerable<float> data, float k = 1.5f)
    {
        // Sort the data
        List<float> sortedData = data.OrderBy(x => x).ToList();
        int n = sortedData.Count;

        // Calculate Q1 and Q3
        float q1 = QuartileCalculator.GetQuartile(sortedData, 0.25f);
        float q3 = QuartileCalculator.GetQuartile(sortedData, 0.75f);

        // Calculate IQR
        float iqr = q3 - q1;

        // Calculate fences
        float lowerFence = q1 - k * iqr;
        float upperFence = q3 + k * iqr;

        // return a tuple containing the fences
        return (lowerFence, upperFence);
    }

    /// <summary>
    ///     Identifies outliers in a given set of data based on the Tukey fences method.
    ///     This can also be used to just count outliers by calling the Count() method on the result.
    ///     Internally, is just uses the CalculateTukeyFences method to determine the fences.
    /// </summary>
    /// <param name="data">The set of data for which to identify outliers.</param>
    /// <param name="k">
    ///     The coefficient to multiply the interquartile range (IQR) by to determine the fence distance. Commonly
    ///     used is 1.5f.
    /// </param>
    /// <returns>
    ///     A list of float values representing the outliers in the given data set (count this if you want to know if
    ///     there are any).
    /// </returns>
    public static List<float> IdentifyOutliers(IEnumerable<float> data, float k = 1.5f)
    {
        // Convert the data to a list
        IEnumerable<float> enumerable = data.ToList();

        // Calculate the Tukey fences
        (float lowerFence, float upperFence) = CalculateTukeyFences(enumerable, k);

        // Return a list of outliers
        return enumerable.Where(x => x < lowerFence || x > upperFence).ToList();
    }
}
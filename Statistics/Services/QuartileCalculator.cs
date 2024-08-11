namespace Statistics.Services;

/// <summary>
///     Provides methods to calculate quartiles from a sorted list of float values.
/// </summary>
public static class QuartileCalculator
{
    /// <summary>
    ///     Calculates the specified "quartileToCalculate" from a sorted list of float values.
    /// </summary>
    /// <param name="sortedList">A sorted list of float values.</param>
    /// <param name="quartileToCalculate">The quartile to calculate (e.g., 0.25 for the first quartile, 0.5 median, etc.).</param>
    /// <returns>The calculated quartile value.</returns>
    public static float GetQuartile(List<float> sortedList, float quartileToCalculate)
    {
        //Sort the list again...just in case
        sortedList.Sort();

        //count the number of elements in the list
        int n = sortedList.Count;

        // Calculate the position of the quartile - (If you are getting different results, this is where approaches sometimes differ)
        float position = (n + 1) * quartileToCalculate;

        // Calculate the lower and upper index
        int lowerIndex = (int)Math.Floor(position) - 1;
        int upperIndex = (int)Math.Ceiling(position) - 1;

        // Calculate the fraction between the lower and upper index
        float fraction = (float)(position - Math.Floor(position));

        // If the lower and upper index are the same, return the value at that index
        if (lowerIndex == upperIndex)
            return sortedList[lowerIndex];

        // Otherwise, calculate the quartile value using linear interpolation
        return sortedList[lowerIndex] * (1 - fraction) + sortedList[upperIndex] * fraction;
    }
}
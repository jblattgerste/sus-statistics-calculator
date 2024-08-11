namespace Statistics.Services;

/// <summary>
///     Represents a system usability scale study (name, study score (average of scores), scores, and raw item scores.
///     So this is basically all the information for one variable (system) in the tool.
/// </summary>
public struct SUSStudy
{
    /// <summary>
    ///     The name of this SUS study (a group of SUS questionnaires: here our variable)
    /// </summary>
    public string SystemName { get; private set; }

    /// <summary>
    ///     The SUS study score (the average of all SUS scores of the sudy)
    /// </summary>
    public float StudyScore { get; }

    /// <summary>
    ///     The standard deviation of the SUS study score
    /// </summary>
    public float StandardDeviation { get; }

    /// <summary>
    ///     The minimum SUS score
    /// </summary>
    public float MinScore { get; }

    /// <summary>
    ///     The maximum SUS score
    /// </summary>
    public float MaxScore { get; }

    /// <summary>
    ///     The first quartile of the SUS scores (Exclusive Median)
    /// </summary>
    public float FirstQuartile { get; set; }

    /// <summary>
    ///     The median of the SUS scores
    /// </summary>
    public float MedianScore { get; }

    /// <summary>
    ///     The third quartile of the SUS scores (Exclusive Median)
    /// </summary>
    public float ThirdQuartile { get; }

    /// <summary>
    ///     A List with all SUS scores
    /// </summary>
    public List<float> SUSScores { get; }

    /// <summary>
    ///     A List of Lists which each hold the 10 raw SUS questionnaire Likert Scale values
    /// </summary>
    public List<List<float>> RawItemScores { get; }

    /// <summary>
    ///     The constructor of the SUS study
    /// </summary>
    /// <param name="systemName">Name of the system (variable)</param>
    /// <param name="rawItemScores">A List of Lists which each hold the 10 raw SUS questionnaire Likert Scale values</param>
    public SUSStudy(string systemName, List<List<float>> rawItemScores)
    {
        // Store the name and raw SUS item data
        SystemName = systemName;
        RawItemScores = rawItemScores;

        // Calculate the SUS scores
        SUSScores = CalculateSusScores(RawItemScores);

        // Calculate the SUS study score
        StudyScore = SUSScores.Average();
        StudyScore = (float)Math.Round(StudyScore, 2);

        // Calculate the sample standard deviation and round it to 2 decimal places
        StandardDeviation = CalculateStandardDeviation(SUSScores);
        StandardDeviation = (float)Math.Round(StandardDeviation, 2);

        // Calculate the min and max
        MinScore = SUSScores.Min();
        MaxScore = SUSScores.Max();

        // Calculate quartiles
        List<float> sortedSusScores = new(SUSScores);
        sortedSusScores.Sort();
        int n = sortedSusScores.Count;

        // Calculate the first quartile, median, and third quartile using the static QuartileCalculator class
        FirstQuartile = QuartileCalculator.GetQuartile(sortedSusScores, 0.25f);
        MedianScore = QuartileCalculator.GetQuartile(sortedSusScores, 0.5f);
        ThirdQuartile = QuartileCalculator.GetQuartile(sortedSusScores, 0.75f);

        return;

        // Helper function to calculate the sample standard deviation of the SUS study score
        //  [ s = \sqrt{\frac{\sum (x_i - \bar{x})^2}{n - 1}} ]
        //
        //  Where:  
        //      ( s ) is the sample standard deviation.
        //      ( x_i ) are the individual data points.
        //      ( \bar{x} ) is the mean of the data points.
        //      ( n ) is the number of data points.
        float CalculateStandardDeviation(List<float> susScores)
        {
            float sum = 0;
            float mean = susScores.Average();
            foreach (float score in susScores)
                sum += (score - mean) * (score - mean);
            return (float)Math.Sqrt(sum / (susScores.Count - 1));
        }
    }


    /// <summary>
    ///     Calculates the SUS scores for a given list of raw item scores.
    /// </summary>
    /// <param name="rawItemScores">A List of Lists containing the raw SUS questionnaire Likert Scale values for each item.</param>
    /// <returns>A List of calculated SUS scores.</returns>
    private static List<float> CalculateSusScores(List<List<float>> rawItemScores)
    {
        // Create a list to store the SUS scores
        List<float> susScores = new();

        // for each row of raw item scores, calculate the SUS score
        foreach (List<float> itemScores in rawItemScores)
        {
            float sum = 0;
            for (int i = 0; i < itemScores.Count; i++)
                if (i % 2 == 0) // Odd-numbered questions (1, 3, 5, 7, 9)
                    sum += itemScores[i] - 1;
                else // Even-numbered questions (2, 4, 6, 8, 10)
                    sum += 5 - itemScores[i];
            float susScore = sum * 2.5f;
            susScores.Add(susScore);
        }

        // return the list of SUS scores
        return susScores;
    }
}
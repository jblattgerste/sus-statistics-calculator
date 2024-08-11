namespace Statistics.Services;

/// <summary>
///     Represents the type of sample dependence of the users data.
/// </summary>
public enum SampleDependence
{
    IndependentSample,
    DependentSample,
    NotSet
}

/// <summary>
///     Represents the type of statistical test to be performed.
/// </summary>
public enum Parametric
{
    Parametric,
    NonParametric,
    NotSet
}

/// <summary>
///     The class that calculates the SUS statistics based on the data provided in the .csv file.
///     This is the brain of the tool holds all data, calculations and user intends.
///     It also calculates the descriptive SUS statistics on construction and stores the results in a list of SUSStudies.
///     Moreover, it performs all the statistical tests (including assumption Tests) on the data by calling the respective
///     services.
/// </summary>
public class SusStatisticsCalculator
{
    /// <summary>
    ///     The constructor used by the .csv file upload. It processes the content of the .csv file with a helper
    ///     function and creates a list of SUSStudy objects.
    /// </summary>
    /// <param name="csvInputString">The entire content of the CSV file (includes the headers still)</param>
    public SusStatisticsCalculator(string csvInputString)
    {
        // Process the content of the CSV file and store it in the Studies property
        Studies = ProcessCSVContent(csvInputString);
    }

    /// <summary>
    ///     Stores whether the user wants to use a parametric or non-parametric test
    /// </summary>
    public Parametric Parametric { get; set; } = Parametric.NotSet;

    /// <summary>
    ///     Stores whether the user has a dependent (paired) or independent sample
    /// </summary>
    public SampleDependence SampleDependence { get; set; } = SampleDependence.NotSet;

    /// <summary>
    ///     The list of SUSStudy objects created from the .csv file.
    ///     They contain all SUS scores, the raw item scores, min, max, median, first and third quartile.
    ///     They also already have the study score calculated with (sample) standard deviation.
    /// </summary>
    public List<SUSStudy> Studies { get; }

    /// <summary>
    ///     The subset of SUSStudy objects that are currently active. This is manipulated by the VariableSelectionService and
    ///     this data is used for the SUS statistics. But this way we also retain the other data for now.
    /// </summary>
    public List<SUSStudy> ActiveStudies { get; set; } = new();


    /// <summary>
    ///     Processes the content of the CSV file and creates a list of SUSStudy objects.
    /// </summary>
    /// <param name="contentString">The entire content of the CSV file (includes the headers still)</param>
    /// <returns>A list of SUSStudy objects</returns>
    private List<SUSStudy> ProcessCSVContent(string contentString)
    {
        // Creat a dictionary to store the studies
        Dictionary<string, List<List<float>>> studies = new();

        // Split the content into lines
        string[] lines = contentString.Split('\n');

        // Iterate over the lines and parse the data by creating a dictionary with the system name as key and the item
        // scores as values. If the system name is not yet in the dictionary, add it, otherwise add the item scores to
        // the existing system name of the dictionary.
        for (int i = 1; i < lines.Length; i++)
        {
            // Trim the line and skip empty lines (important for the last line)
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            // Split the line into values and check if it has exactly 11 columns
            string[] values = line.Split(';');
            if (values.Length != 11)
                continue;

            // Parse the system name and the item scores
            string systemName = values[10];
            List<float> itemScores = values.Take(10).Select(float.Parse).ToList();

            // Add the item scores to the dictionary
            if (!studies.ContainsKey(systemName))
                studies[systemName] = new List<List<float>>();

            // Add the item scores to the existing system name
            studies[systemName].Add(itemScores);
        }

        // Create a list of SUSStudy objects from the dictionary and return it
        return studies.Select(kvp => new SUSStudy(kvp.Key, kvp.Value)).ToList();
    }
}
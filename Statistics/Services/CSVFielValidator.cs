namespace Statistics.Services;

/// <summary>
///     Validates the content of a file containing SUS (System Usability Scale) data.
///     Checks if the file is empty, if it contains a header and at least one row of data, if the header is correct, checks
///     if the data rows have exactly 11 columns, if the ratings are valid (between 1 and 5), and if the System/Variable
///     column is not empty.
/// </summary>
public static class CSVFielValidator
{
    /// <summary>
    ///     Validates the content of a .csv file according to specific rules.
    /// </summary>
    /// <param name="content">The entire content of the .csv file as a string.</param>
    /// <returns>A tuple indicating whether the content is valid and an error message if it is not.</returns>
    public static (bool IsValid, string ErrorMessage) Validate(string content)
    {
        // Split the content into lines
        string[] lines = content.Split('\n');

        // Check if the file is empty
        if (lines.Length == 0)
            return (false, "The provided .csv file is empty.");

        // Check if there's a header and at least one row of data
        if (lines.Length < 3)
            return (false, "The provided file must contain a header and at least one row of data.");

        // Validate the header
        string[] headers = lines[0].Trim().Split(';');
        if (headers.Length != 11 || !headers[0].StartsWith("Question 1") || !headers[9].Equals("Question 10"))
            return (false,
                "The header appears to be incorrect or missing. Expected is that the first row are 11 columns ranging from 'Question 1' to 'Question 10' and ending with the 'System' variable.");

        // Validate all data rows
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            // Skip empty lines - this is important and cant be used for validation because there could be empty lines
            // after the last row so its not necessarily a formatting issue
            if (string.IsNullOrEmpty(line))
                continue;

            // split the line into values
            string[] values = line.Split(';');

            // Check if this row has exactly 11 columns
            if (values.Length != 11)
                return (false,
                    $"The appears to be a problem with the data format. The data in row {i + 1} does not have exactly 11 columns.");

            // Validate Question 1-10 values
            for (int j = 0; j < 10; j++)
                if (!int.TryParse(values[j], out int rating) || rating < 1 || rating > 5)
                    return (false,
                        $"There appears to be at least one invalid item score. In row {i + 1} for question {j + 1}, the value \"{rating}\" was found. Only items scores between 1 and 5 are allowed.");

            // Check if the System column is not empty
            if (string.IsNullOrWhiteSpace(values[10]))
                return (false,
                    $"The System/Variable column is empty in row {i + 1}. Please provide system/variable names so the data can be associated to a variable.");
        }

        // Check if there is at least two unique systems/variables in column 11
        string[] systems = lines.Where(line => !string.IsNullOrWhiteSpace(line))
            .Skip(1)
            .Select(x => x.Split(';')[10])
            .Distinct()
            .ToArray();
        if (systems.Length < 2)
            return (false,
                "There must be at least two unique systems/variables in the provided data set to perform inferential statistics.");

        // Return this tuple if there were no problems found
        return (true, string.Empty);
    }
}
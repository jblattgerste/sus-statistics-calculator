using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Statistics.Components.Helpers;
using Statistics.Services;

namespace Statistics.Pages;

/// <summary>
///     The LoadCSV page is the first page of the SUS Statistics tool. It allows the user to load a CSV file containing
///     the raw SUS data or to manually enter the data. If the user loads data in the form of a CSV file, the data is
///     processed and validated. If the user wants to manually enter the data, they are redirected to the ManualDataEntry
///     page.
/// </summary>
public partial class LoadCSV
{
    /// <summary>
    ///     The SUS data table in raw format, therefore a list of string arrays that represent the rows of the table.
    /// </summary>
    private readonly List<string[]> _rawSusDataTable = [];

    /// <summary>
    ///     Import the associated razor.js file for the LoadCSV page on loading of this component.
    /// </summary>
    private IJSObjectReference? _javascriptJsObjectReference;

    /// <summary>
    ///     A modal window component for displaying file processing error messages on the LoadCSV page.
    /// </summary>
    private ModalWindow? _fileProcessingErrorModalWindow;

    /// <summary>
    ///     The error message displayed in the modal window if the loaded CSV data is not valid.
    /// </summary>
    private string _modalErrorMessage = string.Empty;

    /// <summary>
    ///     This method is called after the component has finished rendering. It imports the associated razor.js file for the
    ///     LoadCSV page and triggers the file loaded event.
    /// </summary>
    /// <param name="firstRender">A boolean value set by Blazor, indicating whether this is the first render of the component.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            _javascriptJsObjectReference = await JavaScript.InvokeAsync<IJSObjectReference>("import",
                "./Pages/LoadCSV.razor.js");
    }

    /// <summary>
    ///     Triggered when a file is loaded in the InputFile field. Handles the file loaded event and processes the loaded
    ///     file into the rawSUSDataTable.
    /// </summary>
    /// <param name="e"> The event arguments of the file loaded event. </param>
    private async Task HandleFileLoad(InputFileChangeEventArgs e)
    {
        // Get the loaded file from the event arguments
        IBrowserFile file = e.File;

        // If the file is not null, check if the file has appropriate size
        await using Stream stream = file.OpenReadStream(1024 * 1024); // This is ca 1MB max

        // Read the file content and split it into lines
        using StreamReader reader = new(stream);
        string content = await reader.ReadToEndAsync();

        // Validate the data using SUSDataValidator
        (bool isValid, _modalErrorMessage) = CSVFielValidator.Validate(content);

        // If the data is not valid, show an error message in the modal window
        if (!isValid)
        {
            if (_fileProcessingErrorModalWindow != null)
            {
                await _fileProcessingErrorModalWindow.Open();
            }
            return;
        }

        // Process the content of the CSV file and create a SusStatisticsCalculator
        SusStatisticsCalculator calculator = new(content);

        // Store the calculator in the GlobalAppState to make it available to other pages globally
        GlobalAppState.Calculator = calculator;

        // Navigate to the VariableSelection page
        NavigationManager.NavigateTo("./VariableSelection");
    }

    /// <summary>
    ///     Uses JavaScript to trigger the file input click event, which opens the file dialog for the user to select a file.
    ///     (The file picker is hidden under a button because it looks weird in PicoCSS and is harder to localize)
    /// </summary>
    private async Task TriggerFilePicker()
    {
        if (_javascriptJsObjectReference != null)
            await _javascriptJsObjectReference.InvokeVoidAsync("triggerFileInputClick", "file-picker");
    }
}
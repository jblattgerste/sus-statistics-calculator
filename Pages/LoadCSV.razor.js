/**
 * Triggers a click event on a file input element with the given element ID.
 * This is used for opening a file dialog when a button is clicked.
 *
 * @param {string} elementId - The ID of the file input element.
 *
 * @return {void}
 */
export function triggerFileInputClick(elementId) {
    const fileInput = document.getElementById(elementId);
    if (fileInput) {
        fileInput.click();
    } else {
        console.error(`Element with id '${elementId}' not found.`);
    }
}
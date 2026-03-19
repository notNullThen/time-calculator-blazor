/**
 * Copies the inner text of an element to the clipboard and provides visual feedback on a button.
 * @param {string} elementId - The ID of the element containing the text to copy.
 * @param {HTMLElement} btn - The button element that was clicked.
 */
window.copyToClipboard = function (elementId, btn) {
    const element = document.getElementById(elementId);
    if (!element) return;

    const text = element.innerText;
    const originalText = btn.innerText;

    const showFeedback = () => {
        btn.innerText = 'Copied!';
        btn.classList.add('btn-success');
        btn.classList.remove('btn-outline-primary');

        setTimeout(() => {
            btn.innerText = originalText;
            btn.classList.remove('btn-success');
            btn.classList.add('btn-outline-primary');
        }, 2000);
    };

    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text)
            .then(showFeedback)
            .catch(err => {
                console.error('Failed to copy text: ', err);
            });
    } else {
        // Fallback for older browsers or non-secure contexts
        const textarea = document.createElement('textarea');
        textarea.value = text;
        // Ensure textarea is not visible but part of the DOM
        textarea.style.position = 'fixed';
        textarea.style.left = '-9999px';
        textarea.style.top = '0';
        document.body.appendChild(textarea);
        textarea.select();
        try {
            document.execCommand('copy');
            showFeedback();
        } catch (err) {
            console.error('Fallback copy failed: ', err);
        }
        document.body.removeChild(textarea);
    }
};

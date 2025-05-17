using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class TerminalManager : MonoBehaviour
{
    public TextMeshProUGUI terminalOutput;  // Reference to the TextMeshProUGUI for terminal output
    public BootSequenceHandler bootSequenceHandler;  // Reference to BootSequenceHandler
    public LoginHandler loginHandler;  // Reference to LoginHandler
    public FileSystemLoader fileSystemLoader;
    public FileSystemManager fileSystemManager;

    public enum TerminalMode { Boot, Login, Shell }
    public TerminalMode currentMode = TerminalMode.Shell;  // Default to Shell mode after login

    public List<string> buffer = new List<string>();
    public string currentInput = "";
    private bool showCursor = true;

    private string playerUsername = "";
    private string playerPassword = "";

    void Start()
    {
        // Start the terminal setup flow by triggering boot and login sequences.
        StartCoroutine(StartTerminal());
    }

    private IEnumerator StartTerminal()
    {
        // Start the boot sequence
        yield return StartCoroutine(bootSequenceHandler.StartBootSequence());

        // After boot, start the login process
        yield return StartCoroutine(loginHandler.StartLoginSequence());
    }

    // --- NEW: Ensure input is captured every frame ---
    void Update()
    {
        HandleInput();
    }

    // Handle user input for the shell mode
    void HandleInput()
    {
        if (currentMode == TerminalMode.Shell)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Handle backspace
                {
                    if (currentInput.Length > 0)
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if (c == '\n' || c == '\r') // Handle command submission
                {
                    SubmitCommand();
                }
                else
                {
                    currentInput += c;
                }
            }
            RenderTerminal(); // Live-update prompt as you type
        }
    }

    // Method to handle user command input
    public void SubmitCommand()
    {
        AddLine($"Command: {currentInput}");  // Output the command typed
        currentInput = "";  // Clear the input after submitting
    }

    // --- NEW: Calculate how many lines fit on the terminal output window ---
    int GetMaxLines()
    {
        // Get the RectTransform height in pixels
        float height = ((RectTransform)terminalOutput.transform).rect.height;
        // Get the font size (in points)
        float fontSize = terminalOutput.fontSize;

        // Optional: Adjust fudge factor for line spacing (1.2 = 20% extra space per line)
        float lineHeight = fontSize * 1.2f;

        // Calculate how many lines fit
        int maxLines = Mathf.FloorToInt(height / lineHeight);
        // Sanity clamp (in case of odd setup)
        return Mathf.Clamp(maxLines, 5, 100);
    }

    // Public method to add a line to the terminal output
    public void AddLine(string line)
    {
        buffer.Add(line);
        while (buffer.Count > GetMaxLines()) // Dynamic visible lines!
            buffer.RemoveAt(0);

        RenderTerminal();
    }

    // Public method to render the terminal output
    public void RenderTerminal()
    {
        string displayPrompt = currentMode == TerminalMode.Shell ? $"{playerUsername}@sbc:~$ " + currentInput : "";
        terminalOutput.text = string.Join("\n", buffer) + "\n" + displayPrompt;
    }

    // Public method to clear the terminal buffer
    public void ClearBuffer()
    {
        buffer.Clear();
    }

    // Public method to change the terminal mode (not needed for login now)
    public void SetMode(TerminalMode mode)
    {
        currentMode = mode;  // Set the mode
        RenderTerminal();     // Re-render the terminal with the new mode
    }
}

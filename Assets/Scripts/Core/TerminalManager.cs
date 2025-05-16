using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public TextMeshProUGUI terminalOutput;

    private List<string> buffer = new List<string>();
    private string currentInput = "";
    private bool showCursor = true;
    private float blinkTimer = 0f;
    private float cursorBlinkRate = 0.5f;
    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;


    // Simple static prompt for now
    private string prompt = "user@localhost:~$ ";

    void Update()
    {
        HandleInput();

        blinkTimer += Time.deltaTime;
        if (blinkTimer >= cursorBlinkRate)
        {
            showCursor = !showCursor;
            blinkTimer = 0;
        }

        RenderTerminal();
    }

    void HandleInput()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (currentInput.Length > 0)
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            else if (c == '\n' || c == '\r')
            {
                SubmitCommand();
            }
            else
            {
                currentInput += c;
            }
        }

        // Handle Up Arrow (previous command)
        if (Input.GetKeyDown(KeyCode.UpArrow) && commandHistory.Count > 0)
        {
            if (historyIndex < 0)
                historyIndex = commandHistory.Count - 1;
            else if (historyIndex > 0)
                historyIndex--;

            currentInput = commandHistory[historyIndex];
        }

        // Handle Down Arrow (next command)
        if (Input.GetKeyDown(KeyCode.DownArrow) && commandHistory.Count > 0)
        {
            if (historyIndex >= 0 && historyIndex < commandHistory.Count - 1)
            {
                historyIndex++;
                currentInput = commandHistory[historyIndex];
            }
            else
            {
                historyIndex = -1;
                currentInput = "";
            }
        }
    }


    void SubmitCommand()
    {
        buffer.Add(prompt + currentInput);

        var outputLines = CommandProcessor.Process(currentInput);

        // If the command was 'clear', clear the buffer instead of adding output
        if (currentInput.Trim().ToLower() == "clear")
        {
            buffer.Clear();
        }
        else
        {
            buffer.AddRange(outputLines);
        }

        // Add non-empty commands to history
        if (!string.IsNullOrWhiteSpace(currentInput))
            commandHistory.Add(currentInput);

        // Reset history position after a command is entered
        historyIndex = -1;
        currentInput = "";
    }


    void RenderTerminal()
    {
        string cursor = showCursor ? "_" : " ";
        terminalOutput.text = string.Join("\n", buffer) + "\n" + prompt + currentInput + cursor;
    }

}

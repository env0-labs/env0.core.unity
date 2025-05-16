using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public TextMeshProUGUI terminalOutput;
    public FileSystemManager fsManager;

    private List<string> buffer = new List<string>();
    private string currentInput = "";
    private bool showCursor = true;
    private float blinkTimer = 0f;
    private float cursorBlinkRate = 0.5f;
    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;
    private enum TerminalMode { Shell, SshUsernamePrompt, SshPasswordPrompt, Login, Password }
    private TerminalMode currentMode = TerminalMode.Shell;

    private string pendingSshIP = "";
    private string pendingSshUser = "";

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
    if (currentMode == TerminalMode.Shell)
    {
        string cwd = fsManager.GetCurrentPath();
        buffer.Add($"user@localhost:{cwd}$ {currentInput}");

        var parts = currentInput.Trim().Split(' ');
        if (parts.Length > 0 && parts[0].ToLower() == "ssh")
        {
            var sshOutput = HandleSshCommand(parts);
            buffer.AddRange(sshOutput);
        }
        else
        {
            var outputLines = CommandProcessor.Process(currentInput, fsManager);

            if (currentInput.Trim().ToLower() == "clear")
            {
                buffer.Clear();
            }
            else
            {
                buffer.AddRange(outputLines);
            }

            if (!string.IsNullOrWhiteSpace(currentInput))
                commandHistory.Add(currentInput);

            historyIndex = -1;
        }

        currentInput = "";
    }
    else if (currentMode == TerminalMode.SshUsernamePrompt)
    {
        buffer.Add("Username: " + currentInput.Trim());
        pendingSshUser = currentInput.Trim();
        currentMode = TerminalMode.SshPasswordPrompt;
        currentInput = "";
    }
    else if (currentMode == TerminalMode.SshPasswordPrompt)
    {
        buffer.Add($"Password for {pendingSshUser}@{pendingSshIP}:");

        bool loginSuccess = TrySshLogin(pendingSshUser, pendingSshIP, currentInput.Trim());
        if (loginSuccess)
        {
            buffer.Add($"Connected to {pendingSshIP} as {pendingSshUser}.");
        }
        else
        {
            buffer.Add("Access denied.");
        }
        currentMode = TerminalMode.Shell;
        currentInput = "";
    }
}




List<string> HandleSshCommand(string[] args)
{
    var output = new List<string>();
    if (args.Length < 2)
    {
        output.Add("Usage: ssh [user@]host");
        return output;
    }

    string arg = args[1];

    if (arg.Contains("@"))
    {
        // ssh user@ip
        var split = arg.Split('@');
        pendingSshUser = split[0];
        pendingSshIP = split[1];
        currentMode = TerminalMode.SshPasswordPrompt;
    }
    else
    {
        // ssh ip
        pendingSshIP = arg;
        pendingSshUser = "";
        currentMode = TerminalMode.SshUsernamePrompt;
    }
    return output;
}

bool TrySshLogin(string username, string ip, string password)
{
    // TODO: Replace this with real network/user check
    // For now: allow login if password is "password"
    return password == "password";
}


void RenderTerminal()
{
    string cursor = showCursor ? "_" : " ";
    string displayPrompt;

    // Only show the shell prompt when in Shell mode.
    if (currentMode == TerminalMode.Shell)
    {
        string cwd = fsManager.GetCurrentPath();
        displayPrompt = $"user@localhost:{cwd}$ " + currentInput + cursor;
    }
    else if (currentMode == TerminalMode.SshUsernamePrompt)
    {
        displayPrompt = "Username: " + currentInput + cursor;
    }
    else if (currentMode == TerminalMode.SshPasswordPrompt)
    {
        displayPrompt = $"Password for {pendingSshUser}@{pendingSshIP}: " + new string('*', currentInput.Length) + cursor;
        // Or if you don't want to echo password chars:
        // displayPrompt = $"Password for {pendingSshUser}@{pendingSshIP}: " + cursor;
    }
    else
    {
        displayPrompt = currentInput + cursor;
    }

if (buffer.Count > 0)
    terminalOutput.text = string.Join("\n", buffer) + "\n" + displayPrompt;
else
    terminalOutput.text = displayPrompt;
}


}


using System.Collections.Generic;

public static class CommandProcessor
{
    public static List<string> Process(string input)
    {
        var output = new List<string>();
        if (string.IsNullOrWhiteSpace(input))
            return output;

        var parts = input.Trim().Split(' ');
        var cmd = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1..] : new string[0];

        switch (cmd)
        {
            case "help":
                output.Add("Available commands: help, echo, clear");
                break;
            case "echo":
                output.Add(string.Join(" ", args));
                break;
            case "clear":
                // We'll handle this specially in TerminalManager
                break;
            default:
                output.Add($"Command not found: {cmd}");
                break;
        }
        return output;
    }
}

using System.Collections.Generic;

public static class CommandProcessor
{
    public static List<string> Process(string input, FileSystemManager fsManager)
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
            output.Add("Available commands: help, echo, clear, ls, cd, cat");
            break;
        case "echo":
            output.Add(string.Join(" ", args));
            break;
        case "ls":
            output.AddRange(fsManager.ListDirectory());
            break;
        case "cd":
            if (args.Length < 1)
                output.Add("Usage: cd <directory>");
            else if (!fsManager.ChangeDirectory(args[0]))
                output.Add("Directory not found.");
            break;
        case "cat":
            if (args.Length < 1)
                output.Add("Usage: cat <filename>");
            else
                output.Add(fsManager.CatFile(args[0]));
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

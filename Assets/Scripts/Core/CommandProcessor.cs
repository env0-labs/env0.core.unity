using System.Collections.Generic;

public static class CommandProcessor
{
    public static List<string> Process(string input, FileSystemManager fsManager)
    {
        var output = new List<string>();
        if (string.IsNullOrWhiteSpace(input))
            return output;

        var parts = input.Trim().Split(' '); // Split the command from the arguments
        var cmd = parts[0].ToLower(); // First part is the command
        var args = parts.Length > 1 ? parts[1..] : new string[0]; // The rest are the arguments

        switch (cmd)
        {
            case "help":
                output.Add("Available commands: help, echo, clear, ls, cd, cat, read");
                break;

            case "echo":
                // Join all arguments and output them
                output.Add(string.Join(" ", args));
                break;

            case "ls":
                // List the contents of the current directory
                string[] directoryContents = fsManager.ListDirectory();
                output.AddRange(directoryContents);
                break;

            case "cd":
                if (args.Length < 1)
                {
                    output.Add("Usage: cd <directory>");
                }
                else if (fsManager.ChangeDirectory(args[0]))
                {
                }
                else
                {
                    output.Add("Directory not found.");
                }
                break;

            case "cat":
                if (args.Length < 1)
                {
                    output.Add("Usage: cat <filename>");
                }
                else
                {
                    string fileContent = fsManager.CatFile(args[0]);
                    output.Add(fileContent); // Display file content
                }
                break;

            case "read":
                if (args.Length < 1)
                {
                    output.Add("Usage: read <filename>");
                }
                else
                {
                    output.Add($"__READ__:{args[0]}"); // This will trigger the modal read in TerminalManager
                }
                break;

            case "clear":
                // Clearing the terminal screen is handled separately in TerminalManager
                break;

            default:
                output.Add($"Command not found: {cmd}");
                break;
        }
        return output;
    }
}

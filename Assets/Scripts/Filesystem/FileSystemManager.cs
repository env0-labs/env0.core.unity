using UnityEngine;
using System.IO;

public class FileSystemManager : MonoBehaviour
{
    [HideInInspector]
    public FileSystemEntry root;  // Root of the filesystem
    private FileSystemEntry currentDirectory;  // Current directory the user is in

    private string filesystemDirectoryPath = "Assets/Resources/Filesystems/";

    void Start()
    {
        // Load filesystem for the current machine/device
        LoadFilesystem(1);  // Example: Load filesystem type 1 for now
    }

    public void LoadFilesystem(int filesystemType)
    {
        string filename = $"filesystem_{filesystemType}.json";
        string path = Path.Combine(filesystemDirectoryPath, filename);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // Detect and handle "structure" wrapper format
            if (json.Contains("\"structure\""))
            {
                var wrapper = JsonUtility.FromJson<FilesystemStructureWrapper>(json);
                if (wrapper != null && wrapper.structure != null && wrapper.structure.root != null)
                {
                    root = wrapper.structure.root;
                }
                else
                {
                    Debug.LogError($"filesystem_{filesystemType}.json: Structure format is invalid.");
                    return;
                }
            }
            else
            {
                root = JsonUtility.FromJson<FileSystemEntry>(json);
            }

            currentDirectory = root;
            SetParents(root, null); // Ensure parent references are set
        }
        else
        {
            Debug.LogError($"Filesystem template {filename} not found.");
        }
    }

    // Helper method to build the current path from the root to currentDirectory
    public string GetCurrentPath()
    {
        return BuildPath(currentDirectory);
    }

    // Recursive method to build the path from the root directory to the current directory
    private string BuildPath(FileSystemEntry entry)
    {
        if (entry == root || entry == null)
            return "/";
        else
            return BuildPath(FindParent(root, entry)) + "/" + entry.name;
    }

    // Helper method to find the parent directory of a file/directory
    private FileSystemEntry FindParent(FileSystemEntry parent, FileSystemEntry child)
    {
        if (parent.contents == null) return null;
        foreach (var e in parent.contents)
        {
            if (e == child) return parent;
            if (e.type == "dir")
            {
                var found = FindParent(e, child);
                if (found != null) return found;
            }
        }
        return null;
    }

    // Get the contents of a file by name
    public string CatFile(string fileName)
    {
        if (currentDirectory.type != "dir") return "(not a directory)";
        foreach (var entry in currentDirectory.contents)
        {
            if (entry.name == fileName && entry.type == "file")
            {
                return entry.content;
            }
        }
        return "File not found.";
    }

    // Set the parent references for directories
    public void SetParents(FileSystemEntry entry, FileSystemEntry parent)
    {
        entry.parent = parent;
        if (entry.type == "dir" && entry.contents != null)
        {
            foreach (var child in entry.contents)
            {
                SetParents(child, entry);
            }
        }
    }

    // List all entries in the current directory
    public string[] ListDirectory()
    {
        // If the current directory is not a directory, return an error message
        if (currentDirectory.type != "dir")
            return new string[] { "(not a directory)" };

        // List all the contents (directories and files) in the current directory
        string[] names = new string[currentDirectory.contents.Length];
        for (int i = 0; i < currentDirectory.contents.Length; i++)
        {
            names[i] = currentDirectory.contents[i].name;
        }
        return names;
    }

    // Change the current directory by name
public bool ChangeDirectory(string path)
{
    // Handle absolute paths
    if (path.StartsWith("/"))
    {
        return ChangeToPath(root, path.Substring(1));  // Strip the leading "/" for absolute path
    }

    // Handle current directory
    if (path == ".")
    {
        return true;
    }

    // Handle parent directory
    if (path == "..")
    {
        if (currentDirectory.parent != null)
        {
            currentDirectory = currentDirectory.parent;
            return true;
        }
        return false; // Already at the root, can't go up
    }

    // Handle all other relative paths
    return ChangeToPath(currentDirectory, path);  // Handle single directory (relative)
}


    // Helper method to change directory based on path
    private bool ChangeToPath(FileSystemEntry start, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            currentDirectory = start;
            return true;
        }

        var parts = path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
        FileSystemEntry dir = start;

        foreach (var part in parts)
        {
            if (part == ".") continue;  // Stay in the same directory
            if (part == "..") // Move up one directory
            {
                if (dir.parent != null) dir = dir.parent;
                continue;
            }

            bool found = false;
            if (dir.type == "dir" && dir.contents != null)
            {
                foreach (var entry in dir.contents)
                {
                    if (entry.name == part && entry.type == "dir")
                    {
                        dir = entry;
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
                return false;  // Directory not found
        }

        currentDirectory = dir;
        return true;  // Successfully changed directory
    }

    // Wrapper classes for structure-style filesystems
    [System.Serializable]
    private class FilesystemStructureWrapper
    {
        public StructureRoot structure;
    }

    [System.Serializable]
    private class StructureRoot
    {
        [HideInInspector]
        public FileSystemEntry root;
    }
}

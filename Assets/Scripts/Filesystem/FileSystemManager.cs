using UnityEngine;

public class FileSystemManager : MonoBehaviour
{
    public FileSystemEntry root;
    private FileSystemEntry currentDirectory;

    public FileSystemManager fsManager;


    void Awake()
    {
        // currentDirectory is set in Start instead of Awake to ensure root is set by loader
    }

    void Start()
    {
    {
        currentDirectory = root;
        SetParents(root, null);
    }
    }

    // ... your ls, cd, cat methods ...

    // List all entries in the current directory
    public string[] ListDirectory()
    {
        if (currentDirectory.type != "dir") return new string[] { "(not a directory)" };
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
    // Handle absolute path
    if (path.StartsWith("/"))
    {
        return ChangeToPath(root, path.Substring(1));
    }

    // Handle parent
    if (path == "..")
    {
        if (currentDirectory.parent != null)
        {
            currentDirectory = currentDirectory.parent;
            return true;
        }
        return false; // Already at root
    }

    // Handle single directory (relative)
    return ChangeToPath(currentDirectory, path);
}

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
        if (part == ".") continue;
        if (part == "..")
        {
            if (dir.parent != null) dir = dir.parent;
            continue;
        }
        var found = false;
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
            return false;
    }
    currentDirectory = dir;
    return true;
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

    // Optionally, expose the current path or dir name
    public string CurrentDirectoryName()
    {
        return currentDirectory.name;
    }

    // Helper method: get current path as a string
    public string GetCurrentPath()
    {
        // This is a simple "stack-less" version, only works if you never cd ..
        // For now, just append names as you traverse from root (works for basic trees)
        return BuildPath(currentDirectory);
    }

    private string BuildPath(FileSystemEntry entry)
    {
        if (entry == root || entry == null)
            return "/";
        else
            return BuildPath(FindParent(root, entry)) + (BuildPath(FindParent(root, entry)) == "/" ? "" : "/") + entry.name;
    }

    // Recursively find parent directory of an entry (only for root->leaf trees)
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
}


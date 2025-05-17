[System.Serializable]

public class FileSystemEntry
{
    public string name;
    public string type; // "file" or "dir"
    public string content; // only used if type == "file"
    public FileSystemEntry[] contents; // only used if type == "dir"

    [System.NonSerialized] // Don't try to serialize/deserialize this field!
    public FileSystemEntry parent;

}

// NOTE for future Ewan (and anyone else):
// Unity may log a harmless warning about serialization depth being exceeded at 'FileSystemEntry.contents'.
// This is SAFE TO IGNORE because we load all data at runtime from JSON and never serialize or display the hierarchy in the Inspector.
// Only revisit if you later want to serialize, inspect, or edit this tree directly in the Unity Editor.

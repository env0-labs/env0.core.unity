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

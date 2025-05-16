using UnityEngine;

public class FileSystemLoader : MonoBehaviour
{
    public TextAsset fileSystemJson;
    public FileSystemEntry root;

    void Awake()
    {
        root = JsonUtility.FromJson<FileSystemEntry>(fileSystemJson.text);

        // Pass the loaded data to the manager
        FileSystemManager manager = GetComponent<FileSystemManager>();
        if (manager != null)
        {
            manager.root = root;
        }

        Debug.Log("Loaded root dir: " + root.name + " with " + root.contents.Length + " entries.");
    }
}

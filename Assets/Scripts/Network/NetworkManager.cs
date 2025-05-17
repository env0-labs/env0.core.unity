using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public DeviceManager deviceManager;  // Reference to DeviceManager
    public FileSystemManager fileSystemManager;  // Reference to FileSystemManager

    void Start()
    {
        // Example IP of the device you want to interact with
        string deviceIp = "10.10.10.1";  // Change this to the IP you want to test

        // Load the device by IP
        Device device = deviceManager.GetDeviceByIp(deviceIp);

        if (device != null)
        {
            Debug.Log($"Device found: {device.name}");
            Debug.Log($"Filesystem type: {device.filesystemType}");

            // Load the filesystem for the device based on the filesystem type
            fileSystemManager.LoadFilesystem(device.filesystemType);

            // Now interact with the device's filesystem
            Debug.Log($"Current directory: {fileSystemManager.GetCurrentPath()}");
        }
        else
        {
            Debug.Log("Device not found!");
        }
    }
}

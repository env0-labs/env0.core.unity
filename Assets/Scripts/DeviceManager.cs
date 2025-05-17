using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class Device
{
    public string name;
    public string ip;
    public int filesystemType;
    public string status;
    public string mac;
    public Dictionary<int, string> openPorts; // ports and status (e.g. {22: "open", 80: "closed"})
}

[System.Serializable]
public class DevicesData
{
    public List<Device> devices;
}

public class DeviceManager : MonoBehaviour
{
    public DevicesData devicesData;

public string devicesFilePath = "devices.json"; // Just use the file name since it's in Resources/ folder

    void Start()
    {
        LoadDevicesData(); // Load devices from the JSON file
    }

    void LoadDevicesData()
    {
        if (File.Exists(devicesFilePath))
        {
            string json = File.ReadAllText(devicesFilePath);
            devicesData = JsonUtility.FromJson<DevicesData>(json);
        }
        else
        {
            Debug.LogError("Devices data file not found!");
        }
    }

    public Device GetDeviceByIp(string ip)
    {
        return devicesData.devices.Find(device => device.ip == ip);
    }

    public List<Device> GetDevicesByFilesystemType(int filesystemType)
    {
        return devicesData.devices.FindAll(device => device.filesystemType == filesystemType);
    }
}

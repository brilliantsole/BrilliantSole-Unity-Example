using System.IO;
using UnityEngine;

public abstract class BS_FileMetadata : ScriptableObject
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_FileMetadata");

    public abstract BS_FileType FileType { get; }
    public string FilePath;
    private byte[] fileData = null;
    private void OnEnable() { fileData = null; }
    public byte[] GetFileData()
    {
        if (fileData != null)
        {
            Logger.Log($"returning existing fileData {fileData.Length} bytes");
            return fileData;
        }

        if (string.IsNullOrEmpty(FilePath))
        {
            Logger.LogError($"Model file path is not set");
            return null;
        }

        string fullPath = Path.Combine(Application.dataPath, FilePath);
        if (!File.Exists(fullPath))
        {
            Logger.LogError($"Model file not found at {fullPath}");
            return null;
        }

        try
        {
            fileData = File.ReadAllBytes(fullPath);
            Logger.Log($"read {fileData.Length} bytes from {fullPath}");
            return fileData;
        }
        catch (IOException ex)
        {
            Logger.LogError($"Error reading model file at {fullPath}: {ex.Message}");
            return null;
        }
    }
}
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public abstract class BS_FileMetadata : ScriptableObject
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_FileMetadata");

    public abstract BS_FileType FileType { get; }
    public string FilePath;
    private byte[] fileData = null;

    private void OnEnable()
    {
        fileData = null;
    }

    public byte[] GetFileData()
    {
        if (fileData != null)
        {
            Logger.Log($"Returning existing fileData {fileData.Length} bytes");
            return fileData;
        }

        if (string.IsNullOrEmpty(FilePath))
        {
            Logger.LogError("Model file path is not set");
            return null;
        }

        string fullPath = Path.Combine(Application.streamingAssetsPath, FilePath);

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using UnityWebRequest request = UnityWebRequest.Get(fullPath);
            request.SendWebRequest();

            while (!request.isDone)
            {
                // Wait for the request to complete
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                fileData = request.downloadHandler.data;
                Logger.Log($"(Android) Read {fileData.Length} bytes from {fullPath}");
                return fileData;
            }
            else
            {
                Logger.LogError($"(Android) Error reading model file at {fullPath}: {request.error}");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"(Android) Unexpected error accessing model file at {fullPath}: {ex.Message}");
            return null;
        }
#else
        // Non-Android platforms can access files directly
        if (!File.Exists(fullPath))
        {
            Logger.LogError($"Model file not found at {fullPath}");
            return null;
        }

        try
        {
            fileData = File.ReadAllBytes(fullPath);
            Logger.Log($"Read {fileData.Length} bytes from {fullPath}");
            return fileData;
        }
        catch (IOException ex)
        {
            Logger.LogError($"Error reading model file at {fullPath}: {ex.Message}");
            return null;
        }
#endif
    }
}

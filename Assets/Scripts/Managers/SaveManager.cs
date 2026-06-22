using UnityEngine;
using System.IO;

public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public void Save(RunData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Saved to: " + SavePath);
    }

    public RunData Load()
    {
        if (!File.Exists(SavePath))
            return null;

        string json = File.ReadAllText(SavePath);

        return JsonUtility.FromJson<RunData>(json);
    }
}

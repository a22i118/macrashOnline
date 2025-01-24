using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;

[System.Serializable]
public class SaveData
{
    public string Controllertype;
    public string Name;
    public string Throw;
    public string Catch;
    public string SpecialAttack;
    public string Jump;
    public string Sleep;
}

public class ConfigSaveManager : MonoBehaviour
{
    private string folderName;
    private void Awake()
    {
        folderName = "save";
        string _savefolderPath = Application.dataPath + "/" + folderName;
        if (!Directory.Exists(_savefolderPath))
        {
            Directory.CreateDirectory(_savefolderPath);
        }

        for (int i = 1; i <= 6; i++)
        {
            if (!File.Exists(_savefolderPath + "/" + $"saveslot{i}.json"))
            {

            }
        }
    }
    private string GetSavePath(int path)
    {
        return Application.dataPath + "/" + folderName + "/" + $"saveslot{path}.json";
    }

    public void KeyConfigSave(SaveData data, int slot)
    {
        string json = JsonUtility.ToJson(data);
        string path = GetSavePath(slot);

        StreamWriter wr = new StreamWriter(path, false);
        wr.WriteLine(json);
        wr.Close();
    }

    public SaveData LoadSaveData(int slot)
    {

        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            return new SaveData();
        }

        string json = File.ReadAllText(path);

        if (string.IsNullOrEmpty(json))
        {
            return new SaveData();
        }

        return JsonUtility.FromJson<SaveData>(json);
    }


    public void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    //public string[] GetAllSaveFiles()
    //{
    //    string[] files = Directory.GetFiles(Application.persistentDataPath, )
    //}
}


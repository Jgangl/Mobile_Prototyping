using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Save_Manager : Singleton<Save_Manager>
{
    const string fileName = "Save.dat";
    string fullSaveFilePath;

    protected override void Awake()
    {
        base.Awake();

        fullSaveFilePath = Application.persistentDataPath + "/" + fileName;
    }

    public bool SaveGame() 
    {
        GameData gameData = new GameData();
        return true;
    }

    public object LoadGame() 
    {
        object loadData = Load(fullSaveFilePath);
        return loadData;
    }

    public bool Save(object saveData) 
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream file = File.Create(fullSaveFilePath);
        formatter.Serialize(file, saveData);
        file.Close();

        return true;
    }

    public object Load(string path) 
    {
        if (!File.Exists(path)) 
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try 
        {
            object saveData = formatter.Deserialize(file);
            file.Close();
            return saveData;
        }
        catch 
        {
            Debug.LogError("Failed to load file at: " + path);
            file.Close();
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data); // Convierte los datos a JSON
        string path = Application.persistentDataPath + "/savefile.json"; // Ruta para guardar
        
        File.WriteAllText(path, json); // Escribe el JSON en el archivo
    }

    public static SaveData LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path); // Lee el archivo JSON
            return JsonUtility.FromJson<SaveData>(json); // Convierte el JSON a un objeto SaveData
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}

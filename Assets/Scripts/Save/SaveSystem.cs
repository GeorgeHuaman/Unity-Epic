using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
public static class SaveSystem
{
    public static void SavePlayer(ProgressLevelSystem progressLevel)
    {
        BinaryFormatter binary = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.fun";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        ProgressDataSystem data = new ProgressDataSystem(progressLevel);

        binary.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static ProgressDataSystem LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter binary = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            ProgressDataSystem progressData = binary.Deserialize(fileStream) as ProgressDataSystem;
            fileStream.Close();
            return progressData;
        }
        else
        {
            Debug.LogError("Save file not found in" + path);
            return null;
        }
    }
}

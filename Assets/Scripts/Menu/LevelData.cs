using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelVersion
{
    public string versionName;
    public Sprite thumbnail;
    [Tooltip("0 = Npcs Kids Genuine y 1 = Npcs Next Old y 2 = Ninguno"),Range(0,2)]
    public int npcs = 2;
}

[CreateAssetMenu(menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public List<LevelVersion> versions;
}
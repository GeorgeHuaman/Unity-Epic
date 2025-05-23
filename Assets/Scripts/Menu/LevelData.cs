using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelVersion
{
    public string versionName;
    public Sprite thumbnail;
}

[CreateAssetMenu(menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public List<LevelVersion> versions;
}
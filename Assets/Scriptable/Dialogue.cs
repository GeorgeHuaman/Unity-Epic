using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<Perfil> response = new List<Perfil>();
    public List<Perfil> questions = new List<Perfil>();
}

[Serializable]
public class Perfil
{
    public Image photo;
    [TextArea(2, 100)] public string response;
}

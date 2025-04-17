using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mision : MonoBehaviour
{
    public string questName;
    public string description;
    public bool startAutomatically;
    public bool taskAreOrdered;
    public List<Task> tasks = new List<Task>();

}

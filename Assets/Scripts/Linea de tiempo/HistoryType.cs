
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryType : MonoBehaviour
{
    public enum ObjectType
    {
        Independencia,
        Revolucion,
        Constitucion,
        Fundacion,
        Llegada
    }
    public ObjectType type;
    public GameObject letter;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectType : MonoBehaviour
{
    public enum Type
    {
        Father,
        Mom,
        Brother,
        Aunt,
        GrandMother
    }
    public Type type;
    public Interactable interactable;
}

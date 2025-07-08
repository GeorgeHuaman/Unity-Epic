using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnims : MonoBehaviour
{
    public List<GameObject> eyesEmotions;

    public void ActivateEyes(int indexToActivate)
    {
        for (int i = 0; i < eyesEmotions.Count; i++)
        {
            eyesEmotions[i].SetActive(i == indexToActivate);
        }
    }

}

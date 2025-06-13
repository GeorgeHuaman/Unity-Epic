using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ManualOclussion : MonoBehaviour
{
    private Vector3 avatar;
    public List<GameObject> objectsToCheck;
    public float distanceToOcclude;
    public float checkInterval = 0.2f;
    void Start()
    {
        //localAvatar = SpatialBridge.actorService.localActor.avatar;
        StartCoroutine(CheckDistanceRoutine());
    }

    private IEnumerator CheckDistanceRoutine()
    {
        while (true)
        {
            //avatar = localAvatar.position;

            foreach (GameObject obj in objectsToCheck)
            {
                if (obj != null)
                {
                    Vector3 positionB = obj.transform.position;
                    float distance = Vector3.Distance(avatar, positionB);

                    if (distance > distanceToOcclude)
                    {
                        obj.SetActive(false);
                    }
                    else
                    {
                        obj.SetActive(true);
                    }
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
}

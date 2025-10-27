using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FragLinea : MonoBehaviour
{
    public HistoryType.ObjectType fragtype;
    public PickHistory pickUp;
    public Transform pos;
    public float moveSpeed = 3f;
    [HideInInspector] public bool completed;
    public Interactable interactable;
    public void TryPlaceObject()
    {
        if (pickUp == null)
            return;

        if (pickUp.currentObject != null && pickUp.currentType == fragtype)
        {
            StartCoroutine(MoveToPosition(pickUp.currentObject.GetComponent<HistoryType>().letter, pos));
            completed = true;
            FinalQuest.instance.AreAllComplete();
            pickUp.Release();
            interactable.enabled = false;
        }
    }

    private IEnumerator MoveToPosition(GameObject obj, Transform target)
    {
        while (obj != null && Vector3.Distance(obj.transform.position, target.position) > 0.05f)
        {
            obj.transform.position = Vector3.Lerp(
                obj.transform.position,
                target.position,
                Time.deltaTime * moveSpeed
            );

            yield return null;
        }

        if (obj != null)
        {
            obj.transform.position = target.position;
            obj.transform.SetParent(target);
        }
    }
}

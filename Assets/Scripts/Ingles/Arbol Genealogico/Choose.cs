using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choose : MonoBehaviour
{
    public ObjectType.Type objectType;
    public Pick pick;
    public float moveSpeed = 3f;
    public Transform pos;
    [HideInInspector] public bool completed;
    public Interactable interactable;

    [Header("Arbol")]
    public GameObject textIncompleted;
    public GameObject textCompleted;
    public void TryPlaceObject()
    {
        if (pick == null)
            return;

        if (pick.currentObject != null && pick.currentType == objectType)
        {
            StartCoroutine(MoveToPosition(pick.currentObject, pos));
            pick.Release();
            completed = true;
            ActivateFinalArbol.instance.AreAllComplete();
            interactable.enabled = false;
            textIncompleted.SetActive(false);
            textCompleted.SetActive(true);
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
            obj.transform.rotation = Quaternion.LookRotation(-target.forward, Vector3.up);
        }
    }
}

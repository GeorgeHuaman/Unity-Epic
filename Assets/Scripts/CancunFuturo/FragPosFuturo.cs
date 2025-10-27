using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragPosFuturo : MonoBehaviour
{
    public FragTypeFuture.ObjectType fragtype;
    public PickFragFuture pickUp;
    public Transform pos;
    public float moveSpeed = 3f;
    [HideInInspector] public bool completed;
    public float rotateSpeed = 60f;
    public Vector3 offset = new Vector3(0, 0.3f, 0);
    public Interactable interactable;
    public void TryPlaceObject()
    {
        if (pickUp == null)
            return;

        if (pickUp.currentObject != null && pickUp.currentType == fragtype)
        {
            StartCoroutine(MoveToPosition(pickUp.currentObject, pos));
            pickUp.Release();
            completed = true;
            ActiveFinalFuture.instance.AreAllComplete();
            interactable.Disable();
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

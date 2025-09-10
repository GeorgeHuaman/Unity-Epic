using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlaceIcon : MonoBehaviour
{
    public Transform pos;
    public float moveSpeed = 1f;
    public float rotateSpeed = 60f;
    public bool isMoving = false;
    public Vector3 offset = new Vector3(0, 0.3f, 0);
    private void Update()
    {
        if (isMoving)
        {
            StartCoroutine(MoveToPosition(pos));
        }
    }
    public void Activate()
    {
        isMoving = true;
    }
    private IEnumerator MoveToPosition(Transform target)
    {
        Quaternion targetRotation = Quaternion.Euler(-180f, target.rotation.eulerAngles.y, 180f);

        while (Vector3.Distance(transform.position, target.position) > 0.05f ||
               Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * moveSpeed);

            transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,rotateSpeed * Time.deltaTime);

            yield return null;
        }


        transform.position = target.position;
        transform.rotation = targetRotation;
        transform.SetParent(target);
        isMoving = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayLength = 100f; // Longitud del Raycast

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            bool hitDetected = Physics.Raycast(ray, out RaycastHit hit, rayLength, layerMask);

            // Dibujar el raycast en la escena
            Color rayColor = hitDetected ? Color.green : Color.red;
            Debug.DrawRay(ray.origin, ray.direction * rayLength, rayColor, 2f);

            if (hitDetected)
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.EnterEvent();
                }

                Debug.Log($"Hit: {hit.collider.name} at {hit.point}");
            }
        }
    }
}
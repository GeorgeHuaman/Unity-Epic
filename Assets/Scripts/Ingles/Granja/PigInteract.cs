using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PigInteract : MonoBehaviour
{
    public Transform destination;
    public float randomRadius = 5f;
    public float maxSampleDistance = 10f;
    private NavMeshAgent navMeshAgent;
    public Interactable interactable;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        if (navMeshAgent == null)
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    public void Interact()
    {

        if (destination != null && navMeshAgent != null)
        {
            Vector3 validDestination = GetRandomValidDestination();
            if (validDestination != Vector3.zero)
            {
                navMeshAgent.SetDestination(validDestination);
            }
            else
            {
                navMeshAgent.SetDestination(destination.position);
            }
        }
        else
        {
            Debug.LogWarning("PigInteract: No se puede mover - destino o NavMeshAgent no asignado");
        }
        interactable.enabled = false;
    }

    private Vector3 GetRandomValidDestination()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * randomRadius;
            randomDirection.y = 0;
            Vector3 randomDestination = destination.position + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDestination, out hit, maxSampleDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return Vector3.zero;
    }
}

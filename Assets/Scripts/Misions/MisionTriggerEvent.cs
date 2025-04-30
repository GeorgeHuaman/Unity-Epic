using UnityEngine;

public class MissionTriggerEvent : MonoBehaviour
{
    public enum TriggerAction
    {
        CompleteTask,
        AddProgressToTask
    }

    [Header("Trigger Settings")]
    public string triggeringTag = "Player";

    [Header("Quest Settings")]
    public Mision targetMission;
    public TriggerAction action;

    [HideInInspector] public int selectedTaskIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggeringTag)) return;
        if (targetMission == null) return;

        Debug.Log($"[Trigger] Activado por: {other.name}");

        int index = selectedTaskIndex;
        if (index < 0 || index >= targetMission.tasks.Count) return;

        var task = targetMission.tasks[index];

        if (targetMission.taskAreOrdered)
        {
            for (int i = 0; i < index; i++)
                if (!targetMission.tasks[i].completed)
                {
                    Debug.Log($"[Trigger] No se puede completar '{task.taskName}' porque la tarea anterior no está completa.");
                    return;
                }
        }

        switch (action)
        {
            case TriggerAction.CompleteTask:
                Debug.Log($"[Trigger] Completando la tarea '{task.taskName}'");
                task.StartTask();
                task.CompleteTask();
                break;

            case TriggerAction.AddProgressToTask:
                Debug.Log($"[Trigger] Añadiendo progreso a la tarea '{task.taskName}'");
                task.AddProgress();
                break;
        }
    }
}
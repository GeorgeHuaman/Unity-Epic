using UnityEngine;
using UnityEngine.Events;

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
    public Mission targetMission;
    public TriggerAction action;

    [HideInInspector] public int selectedTaskIndex;

    [Header("Trigger Events")]
    public UnityEvent onTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggeringTag)) return;

        Debug.Log($"[Trigger] Activado por: {other.name}");

        // Ejecutar UnityEvent asignado
        onTriggerEnterEvent?.Invoke();

        if (targetMission == null) return;

        int index = selectedTaskIndex;
        if (index < 0 || index >= targetMission.tasks.Count) return;

        var task = targetMission.tasks[index];

        if (targetMission.taskAreOrdered)
        {
            for (int i = 0; i < index; i++)
            {
                if (!targetMission.tasks[i].completed)
                {
                    Debug.Log($"[Trigger] No se puede completar '{task.taskName}' porque la tarea anterior no está completa.");
                    return;
                }
            }
        }

        switch (action)
        {
            case TriggerAction.CompleteTask:
                Debug.Log($"[Trigger] Completando la tarea '{task.taskName}'");
                targetMission.CompleteTask(index);
                MissionUI.instance.ShowTaskText();
                break;

            case TriggerAction.AddProgressToTask:
                targetMission.AddProgressToTask(index);
                break;
        }
    }
}
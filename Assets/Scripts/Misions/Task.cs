using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Task
{
    public enum TaskType { Check, Progress }

    public TaskType type;
    public string taskName;

    [ShowIfProgress]
    public int progressSteps;

    public UnityEvent onStart;
    public UnityEvent onComplete;

    [HideInInspector] public bool started = false;
    [HideInInspector] public bool completed;
    [HideInInspector] public int currentProgress = 0;

    public void StartTask()
    {
        if (!started)
        {
            started = true;
            Debug.Log($"[Task] '{taskName}' ha comenzado.");
            onStart?.Invoke();
        }
    }

    public void CompleteTask()
    {
        if (!completed)
        {
            completed = true;
            Debug.Log($"[Task] '{taskName}' ha sido completada.");
            onComplete?.Invoke();
        }
    }

    public void AddProgress()
    {
        if (type != TaskType.Progress || completed) return;

        StartTask();
        currentProgress++;
        Debug.Log($"[Task] '{taskName}' progreso: {currentProgress}/{progressSteps}");

        if (currentProgress >= progressSteps)
        {
            CompleteTask();
        }
    }
}
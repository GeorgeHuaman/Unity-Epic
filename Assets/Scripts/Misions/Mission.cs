using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mission : MonoBehaviour
{
    public string questName;
    [TextArea(5, 20)]
    public string description;
    public bool startAutomatically;
    public bool taskAreOrdered;

    public UnityEvent onMissionStart;
    public UnityEvent onMissionComplete;

    public List<Task> tasks = new List<Task>();

    [ReadOnly]public bool started;
    [ReadOnly]public bool completed;

    private void Start()
    {
        if (startAutomatically)
        {
            StartMission();
        }
    }

    public void StartMission()
    {
        if (!started)
        {
            started = true;
            onMissionStart?.Invoke();
            TryStartNextTask();
        }
    }

    public void CompleteTask(int taskId)
    {
        if (taskId < 0 || taskId >= tasks.Count) return;

        var task = tasks[taskId];

        if (task.completed) return;

        if (taskAreOrdered)
        {
            for (int i = 0; i < taskId; i++)
            {
                if (!tasks[i].completed) return;
            }
        }

        task.StartTask();
        task.CompleteTask();
        TryStartNextTask();

        if (!completed && tasks.TrueForAll(t => t.completed))
        {
            completed = true;
            onMissionComplete?.Invoke();
        }
        MissionUI.instance.ShowTaskText();
    }

    private void TryStartNextTask()
    {
        if (taskAreOrdered)
        {
            foreach (var task in tasks)
            {
                if (!task.started && !task.completed)
                {
                    task.StartTask();
                    break;
                }
            }
        }
    }
    public void AddProgressToTask(int taskIndex)
    {
        if (taskIndex < 0 || taskIndex >= tasks.Count) return;

        var task = tasks[taskIndex];

        if (taskAreOrdered)
        {
            for (int i = 0; i < taskIndex; i++)
            {
                if (!tasks[i].completed) return;
            }
        }

        task.AddProgress();

        if (!completed && tasks.TrueForAll(t => t.completed))
        {
            completed = true;
            onMissionComplete?.Invoke();
        }

        TryStartNextTask();
    }

    public Task CurrentTask
    {
        get
        {
            foreach (var task in tasks)
            {
                if (!task.completed)
                    return task;
            }
            return null;
        }
    }
}

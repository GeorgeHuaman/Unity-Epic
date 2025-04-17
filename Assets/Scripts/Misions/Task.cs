using UnityEngine;

[System.Serializable]
public class Task
{
    public enum TaskType
    {
        Check,
        Progress
    }

    public int id;
    public TaskType type;

    [ShowIfProgress]
    public int progressSteps;
}
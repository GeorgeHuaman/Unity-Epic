using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionTriggerEvent))]
public class MissionTriggerEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MissionTriggerEvent trigger = (MissionTriggerEvent)target;

        trigger.triggeringTag = EditorGUILayout.TextField("Triggering Tag", trigger.triggeringTag);
        trigger.targetMission = (Mision)EditorGUILayout.ObjectField("Target Mission", trigger.targetMission, typeof(Mision), true);
        trigger.action = (MissionTriggerEvent.TriggerAction)EditorGUILayout.EnumPopup("Action", trigger.action);

        if (trigger.targetMission != null && trigger.targetMission.tasks.Count > 0)
        {
            string[] taskNames = new string[trigger.targetMission.tasks.Count];
            for (int i = 0; i < taskNames.Length; i++)
            {
                string name = trigger.targetMission.tasks[i].taskName;
                taskNames[i] = string.IsNullOrEmpty(name) ? $"Task {i}" : name;
            }

            trigger.selectedTaskIndex = EditorGUILayout.Popup("Task", trigger.selectedTaskIndex, taskNames);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a mission with tasks to select a task.", MessageType.Warning);
        }

        // Guarda los cambios en el inspector
        if (GUI.changed)
        {
            EditorUtility.SetDirty(trigger);
        }
    }
}
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionTriggerEvent))]
[CanEditMultipleObjects]
public class MissionTriggerEventEditor : Editor
{
    SerializedProperty triggeringTagProp;
    SerializedProperty targetMissionProp;
    SerializedProperty actionProp;
    SerializedProperty selectedTaskIndexProp;
    SerializedProperty onTriggerEnterEventProp; // <- NUEVO

    private void OnEnable()
    {
        // Vincula las propiedades serializadas
        triggeringTagProp = serializedObject.FindProperty("triggeringTag");
        targetMissionProp = serializedObject.FindProperty("targetMission");
        actionProp = serializedObject.FindProperty("action");
        selectedTaskIndexProp = serializedObject.FindProperty("selectedTaskIndex");
        onTriggerEnterEventProp = serializedObject.FindProperty("onTriggerEnterEvent"); // <- NUEVO
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(triggeringTagProp, new GUIContent("Triggering Tag"));
        EditorGUILayout.PropertyField(targetMissionProp, new GUIContent("Target Mission"));
        EditorGUILayout.PropertyField(actionProp, new GUIContent("Action"));

        // Evento UnityEvent
        EditorGUILayout.PropertyField(onTriggerEnterEventProp, new GUIContent("On Trigger Enter Event")); // <- NUEVO

        // Bot�n para buscar misi�n autom�ticamente
        if (GUILayout.Button("Buscar misi�n en la escena"))
        {
            foreach (var t in targets) // Soporte multiselecci�n
            {
                MissionTriggerEvent trigger = (MissionTriggerEvent)t;
                Mission foundMission = GameObject.FindObjectOfType<Mission>();

                if (foundMission != null)
                {
                    Undo.RecordObject(trigger, "Auto-asignar Misi�n");
                    trigger.targetMission = foundMission;
                    EditorUtility.SetDirty(trigger);
                }
                else
                {
                    Debug.LogWarning($"[{trigger.name}] No se encontr� ninguna misi�n en la escena.");
                }
            }
        }

        // Mostrar selector de tarea solo si hay una misi�n asignada
        foreach (var t in targets)
        {
            MissionTriggerEvent trigger = (MissionTriggerEvent)t;

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
                EditorGUILayout.HelpBox("Asigna una misi�n con tareas para seleccionar una tarea.", MessageType.Warning);
            }

            break; // Solo mostrar el panel de tarea una vez (si hay m�ltiples seleccionados)
        }

        serializedObject.ApplyModifiedProperties();
    }
}
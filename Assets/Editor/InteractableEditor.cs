using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Interactable))]
[CanEditMultipleObjects]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUI.changed)
            SceneView.RepaintAll();
    }

    private void OnSceneGUI()
    {
        Interactable interactable = (Interactable)target; // Manera de saber con que script va ser referencia
        Handles.color = Color.white;

        Vector3 center = interactable.transform.position;
        float radiusInteractable = interactable.interactableRadius;
        float radiusInteractableVisibility = interactable.interactableRadiusVisibility;

        Handles.DrawWireDisc(center,Vector3.up, radiusInteractable);// Dibujo desde donde inicia, en que posicion se crea y cuando es lo que mide
        Handles.DrawWireDisc(center, Vector3.up, radiusInteractableVisibility);

        EditorGUI.BeginChangeCheck();//Metodo para verificar si hubo algun cambio
        Vector3 handlePos = center + Vector3.forward * radiusInteractable;
        handlePos = Handles.Slider(handlePos, Vector3.forward, 0.5f, Handles.ConeHandleCap, 0.01f);// Dibujo el punto (Flecha) desde que posicion hacia que direcion ir, tamaño, forma y velocidad de movimiento
        Vector3 handlePosVisibility = center + Vector3.forward * radiusInteractableVisibility;
        handlePosVisibility = Handles.Slider(handlePosVisibility, Vector3.forward, 0.5f, Handles.ConeHandleCap, 0.01f);
        if (EditorGUI.EndChangeCheck())//metodo para verificar si hubo algun cambio desde que se llamo a BeginChangeCheck
        {
            Undo.RecordObject(interactable, "Change Orbit Radius");// Undo permite poder deshacer cambios con Ctrl + Z
            interactable.interactableRadius = Vector3.Distance(center, handlePos);
            interactable.interactableRadiusVisibility = Vector3.Distance(center, handlePosVisibility);
        }
    }
}

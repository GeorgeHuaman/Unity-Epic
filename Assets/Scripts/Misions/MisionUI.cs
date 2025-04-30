using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public Mision currentMission;
    public TextMeshProUGUI nombreMisionText;
    public TextMeshProUGUI descripcionMisionText;
    public Transform listaDeTareasParent;
    public GameObject tareaPrefab;

    void Start()
    {
        MostrarMision(currentMission);
    }

    public void MostrarMision(Mision mision)
    {
        nombreMisionText.text = mision.questName;
        descripcionMisionText.text = mision.description;

        foreach (Transform child in listaDeTareasParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var task in mision.tasks)
        {
            GameObject nuevaTarea = Instantiate(tareaPrefab, listaDeTareasParent);
            var texto = nuevaTarea.GetComponentInChildren<TextMeshProUGUI>();
            texto.text = $"{task.taskName} - {(task.completed ? "✔️" : "❌")}";
        }
    }
}

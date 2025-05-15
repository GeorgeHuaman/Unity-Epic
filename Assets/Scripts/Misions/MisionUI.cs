using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public static MissionUI instance;
    public Mission mision;
    public TextMeshProUGUI nameMisionText;
    public TextMeshProUGUI descriptionMisionText;
    public TextMeshProUGUI currentTaskText;
    public Animator animator;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        nameMisionText.text = ("Mision: " + mision.questName);
        descriptionMisionText.text = mision.description;
        ShowTaskText();
    }

    public void ShowTaskText()
    {
        Task currentTask = mision.CurrentTask;
        if (currentTask != null)
        {
            currentTaskText.text = currentTask.taskName;
            Debug.Log("[MissionUI] Tarea actual mostrada: " + currentTask.taskName);
        }
        else
        {
            animator.SetBool("Completed", true);
            nameMisionText.text = "¡Misión completada!";
            Debug.Log("[MissionUI] Todas las tareas completadas.");
        }

    }
}

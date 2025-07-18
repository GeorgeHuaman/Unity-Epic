using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    public int sheetRowNumber;

    public List<string> cells = new List<string>();

    // Getters para cada columna:
    public string UserID => cells.Count > 0 ? cells[0] : "";
    public string Password => cells.Count > 1 ? cells[1] : "";
    public string Nombre => cells.Count > 2 ? cells[2] : "";
    public string Apellido => cells.Count > 3 ? cells[3] : "";
    public string Colegio => cells.Count > 4 ? cells[4] : "";
    public string Rol => cells.Count > 5 ? cells[5] : "";
    public string GradoEducativo => cells.Count > 6 ? cells[6] : "";
    public string TiempoDeJuego => cells.Count > 7 ? cells[7] : "";

    // Temas 1…12 están en I (índice 8) hasta T (índice 19)
    public string Tema(int n)
    {
        int idx = 7 + n; // Tema1→8, Tema12→19
        return cells.Count > idx ? cells[idx] : "";
    }

    public void TimeGame(string time)
    {
        cells[7] = time;
    }
    public float Percentage(int tema)
    {
        return float.Parse(cells[tema + 7]);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}

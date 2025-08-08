using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseAlumn : MonoBehaviour
{
    public List<ListExcel> ExcelList = new List<ListExcel>();
    public GoogleSheetsAPI GoogleSheetsAPI;
    public string abeced = "ABCDEFGH";
    public UserSession userSession;
    public static DataBaseAlumn Instance;
    private CreateButtonAlumn CreateButtonAlumn;
    private bool buttonCreate = false;
    public GameObject buttonProfesor;
    public GameObject buttonAlumn;
    private bool init;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        CreateButtonAlumn = CreateButtonAlumn.Instance;
        if(ProgressLevelSystem.instance.currentLevelVersion != null)
        UserSession.Instance.VerifyRol();
    }
    public void CreateList()
    {
        ExcelList.Clear();

        GoogleSheetsAPI.instance.ReadDataFrom("A2", "T");

        foreach (var row in GoogleSheetsAPI.instance.DataFromGoogleSheets.rows)
        {
            if (row.cellData.Count < 7) continue;

            string school = row.cellData[4];
            string rol = row.cellData[5];

            if (UserSession.Instance.cells[4] == school &&
                rol != "Profesor" &&
                UserSession.Instance.cells[5] == "Profesor")
            {
                ListExcel alumno = new ListExcel
                {
                    email = row.cellData[0],
                    password = row.cellData[1],
                    name = row.cellData[2],
                    lastName = row.cellData[3],
                    school = school,
                    rol = rol,
                    gradeEducation = row.cellData[6],
                    gameTime = row.cellData[7]
                };

                for (int i = 8; i < 20 && i < row.cellData.Count; i++)
                {
                    alumno.listProgressTema.Add(row.cellData[i]);
                }

                ExcelList.Add(alumno);
            }
        }

        if (!buttonCreate)
        {
            buttonCreate = true;
            CreateButtonAlumn.CreateButton();
        }
    }

    public void ButtonActive(bool T)
    {
        if (T)
        {
            buttonAlumn.SetActive(false);
        }
        else
        {
            CreateInfoAlumn.Instance.UpdateInfo();
            buttonProfesor.SetActive(false);
        }
    }
}
[Serializable]
public class ListExcel
{
    public string email;
    public string password;
    public string name;
    public string lastName;
    public string school;
    public string rol;
    public string gradeEducation;
    public string gameTime;
    public List<string> listProgressTema = new List<string>();

}

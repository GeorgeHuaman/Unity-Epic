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
    }
    public void CreateList()
    {
        ExcelList.Clear();
        int length = GoogleSheetsAPI.LimitUsser();
        for (int i = 0; i < length; i++)
        {
            if (UserSession.Instance.cells[4].ToString() == GoogleSheetsAPI.FilterEducation("E" +(i+2), "E",i) && GoogleSheetsAPI.FilterEducation("F"+(i+2),"F",i) != "Profesor")
            {
                ExcelList.Add(GoogleSheetsAPI.AddAlumn("A"+(i+2),"T"));
            }
        }
        if (!buttonCreate)
        {
            buttonCreate = true;
            CreateButtonAlumn.CreateButton();
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

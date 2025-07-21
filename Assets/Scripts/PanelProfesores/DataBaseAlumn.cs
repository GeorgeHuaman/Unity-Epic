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

    private void Start()
    {
        CreateList();
    }
    public void CreateList()
    {
        int length = GoogleSheetsAPI.LimitUsser();
        for (int i = 0; i < length; i++)
        {
            if ("Grado" == GoogleSheetsAPI.FilterEducation("E" +(i+2), "E",i))
            {
                ExcelList.Add(GoogleSheetsAPI.AddAlumn("A"+(i+2),"H"));
            }
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
    public List<string> listProgressTema;

}

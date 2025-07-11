using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Google.Apis.Services;
using System;
using System.IO;
using Google.Apis.Sheets.v4.Data;
public class GoogleSheetsAPI : MonoBehaviour
{
    [Header("GoogleSheets Information")]
    [SerializeField] private string spreadSheetID = "1Hiv61NTSyB95qQLL7bG3ATHNgY__SFTKAFZy081mwJU";
    [SerializeField] private string sheetID = "Datos";

    [Header("Data From GoogleSheets")]
    [SerializeField] private string getDataInRange;

    private static SheetsService googleSheetService;

    public RowList DataFromGoogleSheets = new RowList();

    [Header("Write Data From Unity")]
    [SerializeField] private string writeDataInRange;

    public RowList WriteDataFromUnity = new RowList();

    [Header("Delete Data in GoogleSheets")]
    [SerializeField] private string deleteDataInRange;

    void Start()
    {
        var credAsset = Resources.Load<TextAsset>("epicapp-465420-ac43382845f3");
        if (credAsset == null)
        {
            Debug.LogError("[GoogleSheetsAPI] No pude cargar el JSON de credenciales en Resources!");
            return;
        }

        GoogleCredential credential = GoogleCredential
            .FromJson(credAsset.text)
            .CreateScoped(SheetsService.Scope.Spreadsheets);

        // 3) Inicializa el servicio:
        googleSheetService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "GoogleSheets API for Unity"
        });
         
        ReadData();

    }

    public void ReadData()
    {
        string range = sheetID + "!" + getDataInRange;

        var request = googleSheetService.Spreadsheets.Values.Get(spreadSheetID, range);
        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0)
        {
            Debug.Log($"[GoogleSheetsAPI] Total rows read: {values.Count}");
            int rowIndex = 0;

            foreach (var row in values)
            {
                Row newRow = new Row();
                DataFromGoogleSheets.rows.Add(newRow);

                // Construyo un string con todos los valores de esta fila
                var rowDataStrings = new List<string>();
                foreach (var data in row)
                {
                    string cellText = data.ToString();
                    newRow.cellData.Add(cellText);
                    rowDataStrings.Add(cellText);
                }

                // Logueo la fila completa en un solo mensaje
                Debug.Log($"[GoogleSheetsAPI] Row {rowIndex}: {string.Join(", ", rowDataStrings)}");
                rowIndex++;
            }
        }
        else
        {
            Debug.LogWarning("[GoogleSheetsAPI] No se han encontrado datos en el rango especificado.");
        }
    }

    public void WriteData()
    {
        string range = sheetID + "!" + writeDataInRange;
        var valueRange = new ValueRange();
        var cellData = new List<object>();
        var arrows = new List<IList<object>>();
        foreach (var row in WriteDataFromUnity.rows)
        {
            cellData = new List<object>();
            foreach (var data in row.cellData)
            {
                cellData.Add(data);
            }
            arrows.Add(cellData);
        }
        valueRange.Values = arrows;

        var request = googleSheetService.Spreadsheets.Values.Append(valueRange, spreadSheetID, range);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var response = request.Execute();
    }
    
    public void DeleteData()
    {
        string range = sheetID + "!" + deleteDataInRange;
        var deleteData = googleSheetService.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadSheetID, range);
        deleteData.Execute();
    }

    [Serializable]
    public class Row
    {
        public List<string> cellData = new List<string>();
    }
    [Serializable]
    public class RowList
    {
        public List<Row> rows = new List<Row>();
    }
}

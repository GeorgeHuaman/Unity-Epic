using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Services;
using System;
using Google.Apis.Sheets.v4.Data;
using System.Linq;
using UnityEngine.SocialPlatforms;
public class GoogleSheetsAPI : MonoBehaviour
{
    public static GoogleSheetsAPI instance;

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

    private void Awake()
    {
        instance = this; 
    }
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
        ReadDataFrom("A2","A");
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

    public void ReadDataFrom(string startCell, string endCell)
    {
        if (googleSheetService == null)
        {
            Debug.LogError("[GoogleSheetsAPI] Servicio no inicializado.");
            return;
        }

        DataFromGoogleSheets.rows.Clear();
        string range = $"{sheetID}!{startCell}:{endCell}";
        var request = googleSheetService.Spreadsheets.Values.Get(spreadSheetID, range);
        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0)
        {
            for (int i = 0; i < values.Count; i++)
            {
                var row = values[i];
                var newRow = new Row();
                foreach (var cell in row)
                {
                    newRow.cellData.Add(cell.ToString());
                }
                DataFromGoogleSheets.rows.Add(newRow);
                Debug.Log($"[GoogleSheetsAPI] Fila {i}: {string.Join(", ", newRow.cellData)}");
            }
        }
        else
        {
            Debug.LogWarning("[GoogleSheetsAPI] No hay datos en el rango especificado.");
        }
    }

    public List<string> ReadRow(int sheetRowNumber, string startColumn, string endColumn)
    {
        if (googleSheetService == null)
        {
            Debug.LogError("[GoogleSheetsAPI] Servicio no inicializado.");
            return new List<string>();
        }

        // Ej: "Datos!A2:D2"
        string range = $"{sheetID}!{startColumn}{sheetRowNumber}:{endColumn}{sheetRowNumber}";
        var request = googleSheetService.Spreadsheets.Values.Get(spreadSheetID, range);
        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0)
        {
            // values[0] es la lista de celdas de esa fila
            return values[0].Select(cell => cell.ToString()).ToList();
        }

        Debug.LogWarning($"[GoogleSheetsAPI] No encontré datos en la fila {sheetRowNumber}.");
        return new List<string>();
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
    public void WriteDataFor(string startCell, string endCell, params object[] newValues)
    {
        if (googleSheetService == null)
        {
            Debug.LogError("[GoogleSheetsAPI] Servicio no inicializado.");
            return;
        }

        // Construimos el rango completo, p.ej "Datos!C2:D2"
        string range = $"{sheetID}!{startCell}:{endCell}";

        // Preparamos el ValueRange con una sola fila de valores
        var vr = new ValueRange
        {
            Values = new List<IList<object>> { new List<object>(newValues) }
        };

        // Lanzamos el Update
        var updateRequest = googleSheetService
            .Spreadsheets
            .Values
            .Update(vr, spreadSheetID, range);

        updateRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        updateRequest.ExecuteAsync();
        Debug.Log($"[GoogleSheetsAPI] Actualizadas {newValues.Length} celdas en rango {range}");
    }
    public void DeleteData()
    {
        string range = sheetID + "!" + deleteDataInRange;
        var deleteData = googleSheetService.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadSheetID, range);
        deleteData.Execute();
    }
    public void SearchAlumn()
    {
        if(googleSheetService == null)
        {
            Debug.LogError("[GoogleSheetsAPI] Servicio no inicializado.");
            return;
        }

    }
    public void FindLastRowData()
    {
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

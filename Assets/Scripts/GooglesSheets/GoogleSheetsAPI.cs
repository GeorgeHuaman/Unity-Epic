using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Google.Apis.Services;
using System;
using System.IO;
public class GoogleSheetsAPI : MonoBehaviour
{
    [Header("GoogleSheets Information")]
    [SerializeField] private string spreadSheetID = "1Hiv61NTSyB95qQLL7bG3ATHNgY__SFTKAFZy081mwJU";
    [SerializeField] private string sheetID = "Datos";

    [Header("Data From GoogleSheets")]
    [SerializeField] private string getDataInRange;

    private string serviceAccountEmail = "googlesheetsunity@epicapp-465420.iam.gserviceaccount.com";
    private string certificateName = "epicapp-465420-44ec8edf9ddc.p12";
    private string certificatePath;

    private static SheetsService googleSheetService;

    public RowList DataFromGoogleSheets = new RowList();

    void Start()
    {
        var jsonPath = Path.Combine(Application.streamingAssetsPath, "epicapp-465420-ac43382845f3.json");
        GoogleCredential credential;
        using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
        }

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

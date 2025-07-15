using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField cuentaInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("Google Sheets Service")]
    [SerializeField] private GoogleSheetsAPI sheetsApi;
    public void OnLoginButtonPressed()
    {
        sheetsApi.DataFromGoogleSheets.rows.Clear();
        sheetsApi.ReadDataFrom("A2", "B");  // Lee desde A2 hasta la última fila con datos en B

        string user = cuentaInput.text.Trim();
        string pass = passwordInput.text.Trim();
        bool loggedIn = false;
        int rowIndex = 0;

        // 2) Buscar la fila donde coinciden usuario y contraseña
        foreach (var row in sheetsApi.DataFromGoogleSheets.rows)
        {
            if (row.cellData.Count >= 2 &&
                row.cellData[0] == user &&
                row.cellData[1] == pass)
            {
                loggedIn = true;
                break;
            }
            rowIndex++;
        }

        if (!loggedIn)
        {
            Debug.LogWarning("[LoginManager] Usuario o contraseña incorrectos.");
            return;
        }

        Debug.Log("[LoginManager] ¡Login exitoso!");

        // 3) Convertir rowIndex (0‑based) en número de fila real (2‑based)
        int sheetRowNum = rowIndex + 2;

        // 4) Leer toda la fila A–T de esa fila
        List<string> fullRow = sheetsApi.ReadRow(sheetRowNum, "A", "T");

        // 5) Rellenar con cadenas vacías si Sheets no devolvió columnas finales vacías
        int expectedCols = 20; // A→T
        if (fullRow.Count < expectedCols)
        {
            int missing = expectedCols - fullRow.Count;
            for (int i = 0; i < missing; i++)
                fullRow.Add("");
        }

        // 6) Guardar en la sesión
        UserSession.Instance.sheetRowNumber = sheetRowNum;
        UserSession.Instance.cells = fullRow;

        Debug.Log($"[LoginManager] Fila {sheetRowNum} cargada: {string.Join(", ", fullRow)}");
    }
}
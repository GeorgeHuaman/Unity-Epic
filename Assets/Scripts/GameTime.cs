using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public static GameTime instance;
    [SerializeField]private string timeActual;
    [SerializeField] private float tiempoInicial;
    [SerializeField] private float tiempo;
    public bool login = false;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        timeActual = UserSession.Instance.TiempoDeJuego;
        TimeSpan tiempo = TimeSpan.Parse(timeActual);
        tiempoInicial = (float)tiempo.TotalSeconds;
        login = true;
    }
    private void FixedUpdate()
    {
        if (login)
            tiempo += Time.deltaTime;
    }
    private void OnDisable()
    {
        TimeSpan tiempo = TimeSpan.FromSeconds(tiempoInicial+this.tiempo); 
        string tiempoTexto = tiempo.ToString(@"hh\:mm\:ss");
        UserSession.Instance.TimeGame(tiempoTexto); Debug.Log($"Tiempo desde inicio: {tiempoTexto}");
        save(tiempoTexto);
    }
    void save(string time) 
    {
        int fila = UserSession.Instance.sheetRowNumber;
        string celda = "H" + fila;
        GoogleSheetsAPI.instance.WriteDataFor(celda, celda, time);
    }
    void OnApplicationQuit()
    {
        TimeSpan tiempo = TimeSpan.FromSeconds(tiempoInicial); 
        string tiempoTexto = tiempo.ToString(@"hh\:mm\:ss");
        UserSession.Instance.TimeGame(tiempoTexto);
    }
}

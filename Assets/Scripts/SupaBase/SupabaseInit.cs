using Supabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class SupabaseInit : MonoBehaviour
{
    public static Client supabaseClient { get; private set; }

    [Header("Config")]
    public string url = "https://vlkhyeiasecfbuakenfm.supabase.co";
    public string anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZsa2h5ZWlhc2VjZmJ1YWtlbmZtIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTg3MzU2MzksImV4cCI6MjA3NDMxMTYzOX0.jWsRoY3BLkgloBvnbiCZxGGsFYoXlvfAP4BUVDjBgEw";

    async void Start()
    {
        if (supabaseClient != null) return;

        try
        {
            supabaseClient = await InitializeClientWithCallback(url, anonKey);
            Debug.Log("Supabase inicializado (B). Usuario: " + (supabaseClient.Auth.CurrentUser?.Email ?? "no logged"));
        }
        catch (Exception ex)
        {
            Debug.LogError("Error inicializando Supabase (B): " + ex);
        }
    }

    private Task<Client> InitializeClientWithCallback(string url, string key)
    {
        var tcs = new TaskCompletionSource<Client>();

        try
        {
            Client.Initialize(url, key, new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false,
                PersistSession = true
            },
            (client) =>
            {
                tcs.SetResult(client);
            });
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }

        return tcs.Task;
    }
}

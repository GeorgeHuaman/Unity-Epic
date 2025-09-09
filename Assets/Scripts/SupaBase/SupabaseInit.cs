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
    public string url = "https://fkisykairhnoicwucemk.supabase.co";
    public string anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZraXN5a2Fpcmhub2ljd3VjZW1rIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTc0NDA1ODEsImV4cCI6MjA3MzAxNjU4MX0.9M7amF-0Zef0IDlYLP46g39soEVIUkistt8M36mWxZg";

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

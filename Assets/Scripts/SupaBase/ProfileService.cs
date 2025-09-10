using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Postgrest;
using System;

public class ProfileService : MonoBehaviour
{
    public static ProfileService instance { get; private set; }

    public Profile currentProfile { get; private set; }

    public event Action<Profile> onProfileLoad;
    public string Username => currentProfile?.Username;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public async Task<Profile> LoadProfileAsync()
    {
        var client = SupabaseInit.supabaseClient;
        if (client == null)
        {
            Debug.LogWarning("Supabase no inicializado.");
            return null;
        }

        var user = client.Auth.CurrentUser;
        var token = client.Auth.CurrentSession?.AccessToken;

        if (user == null || string.IsNullOrEmpty(token))
        {
            Debug.LogWarning("No hay usuario logueado o token no disponible.");
            return null;
        }

        string myId = user.Id;

        try
        {
            var resp = await client.From<Profile>()
                                   .Filter("id", Postgrest.Constants.Operator.Equals, myId)
                                   .Get();

            var profile = resp.Models.FirstOrDefault();

            currentProfile = profile;
            if (currentProfile != null)
            {
                onProfileLoad?.Invoke(currentProfile);
                //Debug.Log(ProfileService.instance.currentProfile.Username); usaremos esto para cargar datos a futuro.
            }
                

            return currentProfile;
        }
        catch (RequestException rex)
        {
            Debug.LogWarning($"RequestException al obtener profile: {rex.Message}");
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error inesperado al obtener profile: " + ex);
            return null;
        }
    }
    public void Clear()
    {
        currentProfile = null;
        onProfileLoad = null;
    }
}

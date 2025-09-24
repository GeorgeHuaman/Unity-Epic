using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Postgrest;
using System;

public class UsersService : MonoBehaviour
{
    public static UsersService instance { get; private set; }

    public Users currentUser { get; private set; }

    public event Action<Users> onProfileLoad;
    //public string Username => currentProfile?.email;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public async Task<Users> LoadProfileAsync()
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
            var resp = await client.From<Users>()
                                   .Filter("UID", Constants.Operator.Equals, myId)
                                   .Get();

            var profile = resp.Models.FirstOrDefault();

            currentUser = profile;
            if (currentUser != null)
            {
                onProfileLoad?.Invoke(currentUser);
                Debug.Log(currentUser.UID);
                Debug.Log(currentUser.email);
                Debug.Log(currentUser.full_name); //usaremos esto para cargar datos a futuro.
            }
                

            return currentUser;
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
        currentUser = null;
        onProfileLoad = null;
    }
}

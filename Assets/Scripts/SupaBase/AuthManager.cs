using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using TMPro;
using Supabase.Gotrue;
using UnityEngine.Events;
public class AuthManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField cuentaInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("Eventos")]
    public UnityEvent onLoginSuccess;
    public UnityEvent onLoginFailure;

    public async Task<bool> SignIn(string email, string password)
    {
        try
        {
            var client = SupabaseInit.supabaseClient;
            if (client == null)
            {
                Debug.LogError("SupabaseClient no inicializado. Inicializa Supabase antes de usar AuthManager.");
                return false;
            }

            var session = await client.Auth.SignIn(email.Trim(), password.Trim());

            if (session?.User != null)
            {
                Debug.Log($"Logeado: {session.User.Id} ({session.User.Email})");
                //_ = UsersService.instance.LoadProfileAsync();
                onLoginSuccess?.Invoke();
                //_ = TopicProgressService.Instance.LoadProgressForDefaultTopicAsync();
                return true;
            }
            else
            {
                Debug.LogWarning("SignIn no devolvió sesión/usuario. Credenciales inválidas o respuesta inesperada.");
                onLoginFailure?.Invoke();
                return false;
            }
        }
        catch (BadRequestException)
        {
            Debug.LogWarning("Email o contraseña incorrectos.");
            onLoginFailure?.Invoke();
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error inesperado en SignIn: {ex.Message}");
            return false;
        }
    }
    public void SignOut()
    {
        var client = SupabaseInit.supabaseClient;
        if (client == null)
        {
            Debug.LogWarning("SupabaseClient no inicializado.");
            return;
        }

        client.Auth.SignOut();
        Debug.Log("Usuario desconectado.");
    }

    public async void OnSignInButton()
    {
        string user = cuentaInput.text.Trim();
        string pass = passwordInput.text.Trim();
        bool ok = await SignIn(user, pass);
        if (ok)
        {
            Debug.Log("OnSignInButton: login exitoso.");
        }
        else
        {
            Debug.Log("OnSignInButton: credenciales inválidas o error.");
        }
    }
}
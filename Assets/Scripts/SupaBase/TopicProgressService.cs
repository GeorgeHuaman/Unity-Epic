using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Supabase;
using Postgrest;

public class TopicProgressService : MonoBehaviour
{
    public static TopicProgressService Instance { get; private set; }

    public Topic CurrentTopic { get; private set; }
    public List<UserTopicProgress> CurrentProgress { get; private set; }
    public event Action<List<UserTopicProgress>> OnProgressLoaded;

    [Header("Pruebas Supabase")]
    public string slug;
    public short level;
    public int progress;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            string s = string.IsNullOrEmpty(slug) ? "emprendimiento" : slug;
            short lev = level;
            decimal prog = (decimal)progress;
            _ = UpdateLevelProgressBySlug_UpdateOnly(s, lev, prog);
        }
    }
    public async Task<Topic> GetTopicBySlug(string slug)
    {
        var client = SupabaseInit.supabaseClient;
        if (client == null) { Debug.LogError("supabase no inicializado"); return null; }

        try
        {
            var resp = await client.From<Topic>().Filter("slug", Constants.Operator.Equals, slug).Get();

            return resp.Models.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error GetTopicBySlug: " + ex);
            return null;
        }
    }

    public async Task<List<UserTopicProgress>> GetProgressForTopic(string topicId)
    {
        var client = SupabaseInit.supabaseClient;
        var user = client?.Auth?.CurrentUser;
        if (client == null || user == null)
        {
            Debug.LogWarning("Cliente o usuario no disponible.");
            return new List<UserTopicProgress>();
        }

        try
        {
            var resp = await client.From<UserTopicProgress>()
                                   .Filter("user_id", Postgrest.Constants.Operator.Equals, user.Id)
                                   .Filter("topic_id", Postgrest.Constants.Operator.Equals, topicId)
                                   .Order("level", Postgrest.Constants.Ordering.Ascending)
                                   .Get();

            var list = resp.Models?
                           .OrderBy(p => (int)p.Level) // redundante si el servidor ya ordenó, pero seguro
                           .ToList()
                       ?? new List<UserTopicProgress>();

            return list;
        }
        catch (RequestException rex)
        {
            Debug.LogWarning("RequestException GetProgressForTopic: " + rex.Message);
            return new List<UserTopicProgress>();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error GetProgressForTopic: " + ex);
            return new List<UserTopicProgress>();
        }
    }

    public async Task<List<UserTopicProgress>> LoadProgressForDefaultTopicAsync(string slug = "emprendimiento")
    {
        if (Instance == null)
        {
            var go = new GameObject("TopicProgressService");
            Instance = go.AddComponent<TopicProgressService>();
            DontDestroyOnLoad(go);
        }

        // 1) Verificar cliente y sesión
        var client = SupabaseInit.supabaseClient;
        if (client == null)
        {
            Debug.LogWarning("Supabase no inicializado.");
            return null;
        }

        var user = client.Auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("No hay usuario logueado. No se puede cargar progreso.");
            return null;
        }

        try
        {
            // 2) Obtener el topic por slug
            var topic = await GetTopicBySlug(slug);
            if (topic == null)
            {
                Debug.LogWarning($"Topic '{slug}' no encontrado.");
                CurrentTopic = null;
                CurrentProgress = null;
                OnProgressLoaded?.Invoke(CurrentProgress);
                return null;
            }

            CurrentTopic = topic;

            var progress = await GetProgressForTopic(topic.Id);
            if (progress == null)
            {
                Debug.Log("No se encontró progreso o hubo error al obtenerlo.");
                CurrentProgress = null;
                OnProgressLoaded?.Invoke(CurrentProgress);
                return null;
            }

            CurrentProgress = progress;
            OnProgressLoaded?.Invoke(CurrentProgress);

            Debug.Log($"Progress cargado: {progress.Count} niveles para topic '{topic.Name}'.");
            return CurrentProgress;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en LoadProgressForDefaultTopicAsync: " + ex);
            CurrentProgress = null;
            OnProgressLoaded?.Invoke(CurrentProgress);
            return null;
        }
    }
    public async Task<bool> UpdateLevelProgressBySlug_UpdateOnly(string topicSlug, short level, decimal progressValue)
    {
        try
        {
            var topic = await GetTopicBySlug(topicSlug);
            if (topic == null)
            {
                Debug.LogWarning($"UpdateOnly: topic '{topicSlug}' no encontrado.");
                return false;
            }

            var client = SupabaseInit.supabaseClient;
            var user = client?.Auth?.CurrentUser;
            if (client == null || user == null)
            {
                Debug.LogWarning("UpdateOnly: cliente o usuario no disponible.");
                return false;
            }

            string userId = user.Id.ToString();
            string topicId = topic.Id.ToString();
            int levelInt = (int)level;

            var getResp = await client.From<UserTopicProgress>()
                                      .Filter("user_id", Constants.Operator.Equals, userId)
                                      .Filter("topic_id", Constants.Operator.Equals, topicId)
                                      .Filter("level", Constants.Operator.Equals, levelInt)
                                      .Get();

            var existing = getResp?.Models?.FirstOrDefault();
            if (existing == null)
            {
                Debug.Log($"UpdateOnly: No existe fila para user={userId}, topic={topicSlug}, level={level} — nothing to update.");
                return false;
            }

            existing.Progress = progressValue;
            existing.Completed = progressValue >= 100.0M;
            existing.UpdatedAt = DateTime.UtcNow;

            var updateResp = await client.From<UserTopicProgress>()
                                          .Filter("level", Constants.Operator.Equals, levelInt)  
                                         .Update(existing); // pasa la instancia del tipo correcto

            if (updateResp?.Models != null && updateResp.Models.Count > 0)
            {
                Debug.Log($"UpdateOnly: Nivel {level} actualizado para usuario {userId} (topic {topicSlug}).");
                return true;
            }

            Debug.LogWarning("UpdateOnly: la actualización no devolvió filas.");
            return false;
        }
        catch (Postgrest.RequestException rex)
        {
            Debug.LogWarning("UpdateOnly RequestException: " + rex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError("UpdateOnly Error: " + ex);
            return false;
        }
    }
}

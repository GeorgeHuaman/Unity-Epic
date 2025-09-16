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
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    public async Task<bool> UpsertLevelProgress(string topicId, short level, decimal progress)
    {
        var client = SupabaseInit.supabaseClient;
        var user = client?.Auth?.CurrentUser;
        if (client == null || user == null) { Debug.LogWarning("Cliente o usuario no disponible"); return false; }

        try
        {
            var updateModel = new UserTopicProgress
            {
                Progress = progress,
                Completed = (progress >= 100.0M),
                UpdatedAt = DateTime.UtcNow
            };

            var updateResp = await client.From<UserTopicProgress>()
                                         .Filter("user_id", Postgrest.Constants.Operator.Equals, user.Id)
                                         .Filter("topic_id", Postgrest.Constants.Operator.Equals, topicId)
                                         .Filter("level", Postgrest.Constants.Operator.Equals, level)
                                         .Update(updateModel);

            if (updateResp.Models != null && updateResp.Models.Count > 0)
            {
                Debug.Log($"Level {level} updated for user {user.Id} (topic {topicId}).");
                return true;
            }

            //  de prueba, mejor no crear tablas en la version final desde Unity
            var newRow = new UserTopicProgress
            {
                UserId = user.Id,
                TopicId = topicId,
                Level = level,
                Progress = progress,
                Completed = progress >= 100.0M,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResp = await client.From<UserTopicProgress>().Insert(newRow);
            if (insertResp.Models != null && insertResp.Models.Count > 0)
            {
                Debug.Log($"Level {level} inserted for user {user.Id} (topic {topicId}).");
                return true;
            }

            Debug.LogWarning("Upsert did not return any rows.");
            return false;
        }
        catch (Postgrest.RequestException rex)
        {
            Debug.LogWarning("RequestException UpsertLevelProgress: " + rex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error UpsertLevelProgress: " + ex);
            return false;
        }
    }
}

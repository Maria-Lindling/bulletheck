using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameDatabaseClient : MonoBehaviour
{
  [Header("API Base URL (no trailing slash)")]
  [SerializeField] private string baseUrl = "http://localhost/bulletheck_api";

  [Serializable]
  private class SaveRequest { public string name; }

  [Serializable]
  private class SaveResponse { public bool ok; public int id; public string error; }


  [Serializable]
  private class PlayerRow { public int id; public string name; public string created_at; }

  [Serializable]
  private class GetNamesResponse { public bool ok; public PlayerRow[] players; public string error; }

  
  [Serializable]
  private class RecordScoreRequest { public int player1; public int player2 ; public int player1score ; public int player2score ; }

  [Serializable]
  private class RecordScoreResponse { public bool ok; public int id; public string error; }


  [Serializable]
  private class ScoreRow { public int id ; public int player1; public int player2 ; public int player1score ; public int player2score ; public string created_at ; }

  [Serializable]
  private class GetScoresResponse { public bool ok; public ScoreRow[] scores; public string error; }


  public void SaveName(string playerName)
  {
    StartCoroutine( SaveNameCoroutine(playerName) ) ;
  }

  public void FetchNames(Action<int[],string[],string[]> onResult)
  {
    StartCoroutine( GetNamesCoroutine(onResult) ) ;
  }

  public void RecordScore(int player1id, int player2id, int score1, int score2)
  {
    StartCoroutine( RecordScoreCoroutine(player1id, player2id, score1, score2) ) ;
  }


  private IEnumerator RecordScoreCoroutine(int player1id, int player2id, int score1, int score2)
  {
    var url = $"{baseUrl}/record_score.php";

    var reqObj = new RecordScoreRequest() { player1 = player1id, player2 = player2id, player1score = score1, player2score = score2 } ;

    string json = JsonUtility.ToJson(reqObj);
    byte[] body = Encoding.UTF8.GetBytes(json);

    using var req = new UnityWebRequest(url, "POST");
    req.uploadHandler = new UploadHandlerRaw(body);
    req.downloadHandler = new DownloadHandlerBuffer();
    req.SetRequestHeader("Content-Type", "application/json");

    yield return req.SendWebRequest();

    if (req.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"RecordScore failed: {req.error}");
        yield break;
    }

    var res = JsonUtility.FromJson<SaveResponse>(req.downloadHandler.text);
    if (res == null || !res.ok)
    {
        Debug.LogError($"RecordScore server error: {(res != null ? res.error : "Invalid JSON")}");
        yield break;
    }

    Debug.Log($"Recorded score of {score1}|{score2} with id={res.id}") ;
  }

  private IEnumerator SaveNameCoroutine(string playerName)
  {
    var url = $"{baseUrl}/save_name.php";

    var reqObj = new SaveRequest { name = playerName };
    string json = JsonUtility.ToJson(reqObj);
    byte[] body = Encoding.UTF8.GetBytes(json);

    using var req = new UnityWebRequest(url, "POST");
    req.uploadHandler = new UploadHandlerRaw(body);
    req.downloadHandler = new DownloadHandlerBuffer();
    req.SetRequestHeader("Content-Type", "application/json");

    yield return req.SendWebRequest();

    if (req.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"SaveName failed: {req.error}");
        yield break;
    }

    var res = JsonUtility.FromJson<SaveResponse>(req.downloadHandler.text);
    if (res == null || !res.ok)
    {
        Debug.LogError($"SaveName server error: {(res != null ? res.error : "Invalid JSON")}");
        yield break;
    }

    Debug.Log($"Saved name '{playerName}' with id={res.id}") ;
  }

  private IEnumerator GetNamesCoroutine(Action<int[],string[],string[]> onResult)
  {
    var url = $"{baseUrl}/get_names.php";

    using var req = UnityWebRequest.Get(url);
    yield return req.SendWebRequest();

    if (req.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"FetchNames failed: {req.error}");
        onResult?.Invoke(Array.Empty<int>(),Array.Empty<string>(),Array.Empty<string>());
        yield break;
    }

    var res = JsonUtility.FromJson<GetNamesResponse>(req.downloadHandler.text);
    if (res == null || !res.ok || res.players == null)
    {
        Debug.LogError($"FetchNames server error: {(res != null ? res.error : "Invalid JSON")}");
        onResult?.Invoke(Array.Empty<int>(),Array.Empty<string>(),Array.Empty<string>());
        yield break;
    }

    int[] ids = new int[res.players.Length];
    for (int i = 0; i < res.players.Length; i++)
        ids[i] = res.players[i].id;

    string[] names = new string[res.players.Length];
    for (int i = 0; i < res.players.Length; i++)
        names[i] = res.players[i].name;

    string[] created_at = new string[res.players.Length];
    for (int i = 0; i < res.players.Length; i++)
        created_at[i] = res.players[i].created_at;

    onResult?.Invoke(ids,names,created_at);
  }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FishNet.Authenticating;
using FishNet.Object;
using GameKit.Dependencies.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Networking;

public class GameDatabaseClient : NetworkBehaviour
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
  private class RecordScoreRequest { public int player1; public int player2 ; public int score ; }

  [Serializable]
  private class RecordScoreResponse { public bool ok; public int id; public string error; }


  [Serializable]
  private class ScoreRow { public int id ; public int player1; public int player2 ; public int score ; public string created_at ; }


  [Serializable]
  private class GetScoresResponse { public bool ok; public ScoreRow[] scores; public string error; }


  [Serializable]
  private class GetScoreRequest { public int id; }

  [Serializable]
  private class GetScoreResponse { public bool ok; public ScoreRow score; public string error; }

  private readonly List<(string PlayerName, int Id)> _registeredPlayers = new() ;
  private readonly List<ScoreRow> _allScores = new () ;


  public void SaveName(string playerName)
  {
    StartCoroutine( SaveNameCoroutine(playerName, (pName,pId) => { _registeredPlayers.Add( (playerName,pId) ) ; } ) ) ;
  }

  public void FetchNames(Action<int[],string[],string[]> onResult)
  {
    StartCoroutine( GetNamesCoroutine(onResult) ) ;
  }

  public void RecordScore(int player1id, int player2id, int score, Action<bool,int,string> onResult = null)
  {
    StartCoroutine( RecordScoreCoroutine(player1id, player2id, score, onResult) ) ;
  }

  private void FetchScore(int id, Action<ScoreRow> onResult = null)
  {
    StartCoroutine( FetchScoreCoroutine(id,onResult) ) ;
  }

  private void FetchScores(Action<ScoreRow[]> onResult = null)
  {
    StartCoroutine( FetchScoresCoroutine(onResult) ) ;
  }


  private IEnumerator FetchScoresCoroutine(Action<ScoreRow[]> onResult)
  {
    var url = $"{baseUrl}/get_scores.php";

    using var req = UnityWebRequest.Get(url);
    yield return req.SendWebRequest();

    if (req.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"RecordScore failed: {req.error}");
        yield break;
    }

    var res = JsonUtility.FromJson<GetScoresResponse>(req.downloadHandler.text);
    if (res == null || !res.ok)
    {
        Debug.LogError($"RecordScore server error: {(res != null ? res.error : "Invalid JSON")}");
        yield break;
    }

    onResult?.Invoke( res.scores ) ;
  }

  private IEnumerator FetchScoreCoroutine(int id, Action<ScoreRow> onResult)
  {
    var url = $"{baseUrl}/get_score.php";

    var reqObj = new GetScoreRequest() { id = id } ;

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
    
    var res = JsonUtility.FromJson<GetScoreResponse>(req.downloadHandler.text);
    if (res == null || !res.ok)
    {
        Debug.LogError($"RecordScore server error: {(res != null ? res.error : "Invalid JSON")}");
        yield break;
    }

    onResult?.Invoke( res.score ) ;
  }

  private IEnumerator RecordScoreCoroutine(int player1id, int player2id, int score, Action<bool,int,string> onResult)
  {
    var url = $"{baseUrl}/record_score.php";

    var reqObj = new RecordScoreRequest() { player1 = player1id, player2 = player2id, score = score } ;

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

    var res = JsonUtility.FromJson<RecordScoreResponse>(req.downloadHandler.text);
    if (res == null || !res.ok)
    {
        Debug.LogError($"RecordScore server error: {(res != null ? res.error : "Invalid JSON")}");
        yield break;
    }

    Debug.Log($"Recorded score of {score} with id={res.id}") ;

    onResult?.Invoke( res.ok, res.id, res.error ) ;
  }

  private IEnumerator SaveNameCoroutine(string playerName,Action<string,int> onResult)
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

    onResult?.Invoke( playerName, res.id ) ;
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


#region 
  public IEnumerator SubmitScore(string host, string client, int score)
  {
    int host_id = -1 ;
    int client_id = -1 ;

    if( !_registeredPlayers.Any( (item) => {
      if( host == item.PlayerName )
      {
        host_id = item.Id ;
        return true ;
      }
      return false ;
    } ) )
    {
      yield return StartCoroutine( SaveNameCoroutine(host, (name,id) => { _registeredPlayers.Add( (host,id) ) ; host_id = id ; } ) ) ;
    }

    if( !_registeredPlayers.Any( (item) => {
      if( client == item.PlayerName )
      {
        client_id = item.Id ;
        return true ;
      }
      return false ;
    } ) )
    {
      yield return StartCoroutine( SaveNameCoroutine(client, (name,id) => { _registeredPlayers.Add( (client,id) ) ; client_id = id ; } ) ) ;
    }

    RecordScore( host_id, client_id, score, (ok,id,error) => { FetchScore(id, (sRow) => { _allScores.Add(sRow) ; } ) ; } ) ;
  }
#endregion


#region Game Events
  public void OnSubmitScoreToDb(GameEventContext ctx)
  {
    StartCoroutine(
      SubmitScore(
        ctx.ReadValue<string>(0),
        ctx.ReadValue<string>(1),
        ctx.ReadValue<int>(0)
      )
    ) ;
  }

  public void OnRefreshLeaderboardEntries(GameEventContext ctx)
  {
    Transform parent = ctx.Source.GetComponent<GameFinishMenuOverlay>().LeaderboardTransform ;

    int i = 0 ;
    foreach(var (host, client, score) in AllScores().Take(parent.childCount))
    {
      parent.transform.GetChild(i).GetComponent<LeaderboardEntry>().SetValues(host, client, score) ; 
      i++ ;
    }
  }
#endregion


#region 
  public (string host, string client, int score)[] AllScores()
  {
    List<(string,string,int)> values = new () ;

    foreach(ScoreRow scoreRow in _allScores)
    {
      values.Add(
        (
          _registeredPlayers.First( (kvp) => { return kvp.Id == scoreRow.player1 ; } ).PlayerName,
          _registeredPlayers.First( (kvp) => { return kvp.Id == scoreRow.player2 ; } ).PlayerName,
          scoreRow.score
        )
      ) ;
    }

    return values.OrderByDescending((entry)=>entry.Item3).ToArray() ;
  }
#endregion


  private void Start()
  {
    if( !IsServerInitialized )
    {
      Destroy( gameObject ) ;
      return ;
    }
    
    FetchNames( (ids,names,created_ats) => {
      for( int i = 0 ; i < ids.Length ; i++ )
      {
        _registeredPlayers.Add( ( names[i], ids[i] ) ) ;
      }
    } ) ;

    FetchScores( (scoreRows) => {
      for( int i = 0 ; i < scoreRows.Length ; i++ )
      {
        _allScores.Add( scoreRows[i] ) ;
      } ;
    } ) ;
  }
}
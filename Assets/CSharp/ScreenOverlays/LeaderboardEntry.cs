
using System.Collections;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

// syncVar values are not updating on clients for [a reason]
public class LeaderboardEntry : NetworkBehaviour
{
  [SerializeField] private TextMeshProUGUI hostText ;
  [SerializeField] private TextMeshProUGUI clientText ;
  [SerializeField] private TextMeshProUGUI scoreText ;

  private readonly SyncVar<string> syncVarHost = new () ;
  private readonly SyncVar<string> syncVarClient = new () ;
  private readonly SyncVar<int> syncVarScore = new () ;

  public void SetValues(string host, string client, int score)
  {
    if( !IsServerInitialized )
      return ;
    syncVarHost.Value = host ;
    syncVarClient.Value = client ;
    syncVarScore.Value = score ;
  }

  void OnHostChange(string prev, string next, bool isServer)
  {
    var localClient = GameObject.FindObjectsByType<ClientShell>(FindObjectsSortMode.None).FirstOrDefault( (cs) => cs.IsOwner ) ;
    if( !isServer && localClient.Seat == PlayerSelect.Player2 )
      Debug.Log( $"Local client {localClient.Seat} updating host leaderboard entry from {prev} to {next}" ) ;

    if( prev == next )
      return ;
      
    hostText.text = next ;
  }

  void OnClientChange(string prev, string next, bool isServer)
  {
    if( prev == next )
      return ;

    clientText.text = next ;
  }

  void OnScoreChange(int prev, int next, bool isServer)
  {
    if( prev == next )
      return ;

    scoreText.text = next.ToString("###,###,###,##0") ;
  }

  void OnDestroy()
  {
    syncVarHost.OnChange   -= OnHostChange;
    syncVarClient.OnChange -= OnClientChange;
    syncVarScore.OnChange  -= OnScoreChange;
  }

  void Awake()
  {
    StartCoroutine( DelayedInit() ) ;
  }

  IEnumerator DelayedInit()
  {
    yield return null ;
    syncVarHost.OnChange   += OnHostChange;
    syncVarClient.OnChange += OnClientChange;
    syncVarScore.OnChange  += OnScoreChange;
  }
}
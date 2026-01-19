
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

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
    hostText.text = next ;
  }

  void OnClientChange(string prev, string next, bool isServer)
  {
    clientText.text = next ;
  }

  void OnScoreChange(int prev, int next, bool isServer)
  {
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
    syncVarHost.OnChange   += OnHostChange;
    syncVarClient.OnChange += OnClientChange;
    syncVarScore.OnChange  += OnScoreChange;
  }

  void Start()
  {
  }
}
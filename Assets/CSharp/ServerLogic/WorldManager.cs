using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

[RequireComponent(typeof(GameDatabaseClient))]
public class WorldManager : NetworkBehaviour
{
#region UnityEditor
  [Header("UI")]
  // [SerializeField] private TMP_Text stateText ;
  // [SerializeField] private TMP_Text player1NameText ;
  // [SerializeField] private TMP_Text player2NameText ;
  // [SerializeField] private TMP_InputField playerNameField ;
  // [SerializeField] private Button readyButton ;

  public readonly SyncVar<string> player1Name = new() ;
  public readonly SyncVar<string> player2Name = new() ;

  [Header("Score")]
  private readonly SyncVar<int> scoreP1 = new() ;
  private readonly SyncVar<int> scoreP2 = new() ;

  [Header("Game")]
  private readonly SyncVar<GameState> gameState = new() ;
  private readonly SyncVar<Color> ballColor = new() ;
#endregion


#region db
  private GameDatabaseClient _gameDatabaseClient ; 
#endregion


#region Events
#endregion


#region MonoBehavior
  private void Awake() { }

  private void Update() { }
#endregion


#region NetworkBehavior
  public override void OnStartServer()
  {
    base.OnStartServer() ;    
  }
#endregion
}

using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(GameDatabaseClient))]
public class WorldManager : NetworkBehaviour
{
#region UnityEditor
  [Header("Motion & Parallax")]
  [SerializeField] private GameObject sceneBackdrop ;
  [SerializeField] private float verticalScrollSpeed = 0.3334f ;
  [SerializeField] private float lookParallaxIntensity = 1.0f ;

  [Header("UI")]
  // [SerializeField] private GameObject sceneBackdrop ;
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
  public void OnPauseMenu(GameEventContext ctx)
  {
    switch( gameState.Value )
    {
      case GameState.WaitingForPlayers :
        // DUMMY / TEST 
        //gameState.Value = GameState.WaitingPaused ;
        gameState.Value = GameState.Playing ;
        break ;

      case GameState.WaitingPaused :
        gameState.Value = GameState.WaitingForPlayers ;
        break ;

      case GameState.Playing :
        gameState.Value = GameState.Paused ;
        break ;

      case GameState.Paused :
        gameState.Value = GameState.Playing ;
        break ;
      
      case GameState.Finished :
        // nothing
        break ;
    }
  }
#endregion


#region Backdrop
  private void ScrollBackdrop()
  {
    if( gameState.Value != GameState.Playing )
      return ;

    sceneBackdrop.transform.position = new Vector3(
      sceneBackdrop.transform.position.x,
      sceneBackdrop.transform.position.y - verticalScrollSpeed * Time.deltaTime
    ) ;
  }
#endregion


#region OnTick
  private void OnTick()
  {
    if( !IsServerInitialized )
      return ;

    ScrollBackdrop() ;
  }
#endregion


#region MonoBehavior
  private void Awake() { }
  private void Update() { }
#endregion


#region NetworkBehavior
  public override void OnStartServer()
  {
    base.OnStartServer() ;

    sceneBackdrop.SetActive( true ) ;

    if( TimeManager != null )
    {
      TimeManager.OnTick += OnTick ;
    }
  }
#endregion
}

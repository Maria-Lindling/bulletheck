using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using System;
using System.Collections;
using FishNet.Example.Scened;

// main
// TODO: add "retry level" button
// TODO: add "fail and retry/exit" panel to game finish menu
// TODO: finish game finish menu integration / logic
// TODO: hook up game finish menu to php / database
// misc
// TODO: finishing game / open menu disables player input on vessel
// TODO: populate leaderboard
// TODO: alternate leaderboard view with "your score" for comparison
[RequireComponent(typeof(GameDatabaseClient))]
public class WorldManager : NetworkBehaviour, IEntityController
{
#region UnityEditor
  [Header("Players")]
  [SerializeField] private GameObject playerVessel ;
  [SerializeField] private GameObject playerSpawnPoint ;
  [SerializeField] private GameObject clientLogicObject ;

  [Header("Encounter")]
  [SerializeField] private List<GameObject> enemySpawnPoints ;
  [SerializeField] private List<EncounterScriptableObject> encounters ;
  [SerializeField] private int damagePenalty = 5000 ;

  [Header("Motion & Parallax")]
  [SerializeField] private GameObject sceneBackdrop ;
  [SerializeField] private float verticalScrollSpeed = 0.3334f ;
  //[SerializeField] private float lookParallaxIntensity = 1.0f ;

  //[Header("UI")]
  // [SerializeField] private GameObject sceneBackdrop ;
  // [SerializeField] private TMP_Text player1NameText ;
  // [SerializeField] private TMP_Text player2NameText ;
  // [SerializeField] private TMP_InputField playerNameField ;
  // [SerializeField] private Button readyButton ;

  public readonly SyncVar<string> player1Name = new() ;
  public readonly SyncVar<string> player2Name = new() ;

  [Header("Score")]
  private readonly SyncVar<int> syncPlayerScore = new() ;

  [Header("Game")]
  private readonly SyncVar<GameState> gameState = new() ;
#endregion


#region db
  private GameDatabaseClient _gameDatabaseClient ; 
#endregion


#region 
  private Queue<EncounterScriptableObject> _encountersQueue ;
  private readonly List<ClientShell> _connectedPlayers = new() ;
#endregion


#region 
  public ForceSelection Force => ForceSelection.None ;
#endregion


  private void CheckScenarioBegin()
  {
    if( gameState.Value == GameState.WaitingForPlayers && _connectedPlayers.Count < 2 )
    {
      
      GameEventContextBuilder gCtxB = new( gameObject ) ;
      gCtxB.AddValue<string>("Waiting for co-op partner.") ;
      GameEventSystem.SetMessage.Invoke( gCtxB.Build() ) ;
      GameEventSystem.ShowMessage.Invoke( new(gameObject) ) ;
      return ;
    }
    GameEventContext vesselCtx = new GameEventContextBuilder( gameObject )
      .AddValue<GameObject>(playerVessel)
      .AddValue<Vector3>(playerSpawnPoint.transform.position)
      .Build() ;

    GameEventSystem.HideMessage.Invoke( new(gameObject) ) ;
    GameEventSystem.SpawnVessel.Invoke( vesselCtx ) ;
  }


  #region Events
  public void OnClientConnect(GameEventContext ctx)
  {
    if( ctx.Source.TryGetComponent<ClientShell>(out ClientShell clientShell))
    {
      _connectedPlayers.Add( clientShell ) ;
      clientShell.AssignSeat( (PlayerSelect) _connectedPlayers.Count ) ;
      ctx.Source.transform.SetParent( clientLogicObject.transform ) ;
    }

    CheckScenarioBegin() ;
  }

  public void OnVesselSpawned(GameEventContext ctx)
  {
    if( ctx.Source.TryGetComponent<PlayerVessel>(out PlayerVessel playerVessel))
    {
      foreach( ClientShell playerShell in _connectedPlayers)
      {
        playerShell.AssignVessel( playerVessel ) ;
      }

      sceneBackdrop.SetActive( true ) ;
      gameState.Value = GameState.Playing ;
      GameEventSystem.ScenarioBegin.Invoke( new GameEventContext( gameObject ) ) ;
    }
  }

  // bool value = defeated? true/false
  public void OnVesselDespawned(GameEventContext ctx)
  {
    if( ctx.TryReadValue<bool>(out bool value) && value == true )
    {
      GameEventContextBuilder gCtxB = new( gameObject ) ;
      gCtxB.AddValue<string>( "Bad luck!\n\nTry Again?" ) ;
      GameEventSystem.SetMessage.Invoke( gCtxB.Build() ) ;
      GameEventSystem.ShowMessage.Invoke( new(gameObject) ) ;
    }
  }

  public void OnScenarioBegin(GameEventContext ctx)
  {
    _encountersQueue = new ( encounters ) ;

    gameState.Value = GameState.Playing ;

    StartCoroutine( RollOutEncounters() ) ;
  }

  public void OnEncounterEnd(GameEventContext ctx)
  {
    gameState.Value = GameState.Finished ;
  }

  public void OnPauseMenu(GameEventContext ctx)
  {
    // DEBUG: cheat victory
    GameEventSystem.EncounterEnd.Invoke( new GameEventContextBuilder( gameObject ).AddValue<int>(14069).Build() ) ;

    switch( gameState.Value )
    {
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

  public void OnPlayerTakeDamage( GameEventContext ctx )
  {
    if( gameState.Value != GameState.Playing )
      return ;

    syncPlayerScore.Value = Math.Max( syncPlayerScore.Value - damagePenalty, 0 ) ;
  }

  public void OnEnemyDefeated( GameEventContext ctx )
  {
    if( gameState.Value != GameState.Playing )
      return ;

    if( ctx.TryReadValue<int>(out int value) )
    {
      syncPlayerScore.Value += value ;
    }
  }
#endregion


#region 
  private void OnScoreChange(int prev, int next, bool isServer)
  {
    GameEventSystem.ScorePoint.Invoke(
      new GameEventContextBuilder( gameObject )
        .AddValue<int>( next )
        .Build()
    ) ;
  }

  private void OnGameStateChange(GameState prev, GameState next, bool isServer)
  {
    switch( next )
    {
      case GameState.Playing :
        Time.timeScale = 1.0f ;
        break ;

      case GameState.Paused :
        Time.timeScale = 0.0f ;
        break ;
      
      case GameState.Finished :
        // nothing
        break ;
    }
  }
#endregion


#region Encounter
  private IEnumerator RollOutEncounters()
  {
    while( _encountersQueue.Count > 0 && gameState.Value == GameState.Playing )
    {
      EncounterScriptableObject nextEncounter = _encountersQueue.Dequeue() ;

      Queue<EncounterEntry> encounterQueue = new (nextEncounter.Data) ;

      StartCoroutine( RollOutEncounter(encounterQueue) ) ;
      
      if( gameState.Value == GameState.Playing )
        yield return new WaitForSeconds( nextEncounter.Duration ) ;
    }

    if( gameState.Value == GameState.Playing )
      GameEventSystem.EncounterEnd.Invoke( new GameEventContextBuilder( gameObject ).AddValue<int>(syncPlayerScore.Value).Build() ) ;
  }

  private IEnumerator RollOutEncounter(Queue<EncounterEntry> encounterQueue)
  {
    while( encounterQueue.Count > 0 && gameState.Value == GameState.Playing )
    {
      EncounterEntry entry = encounterQueue.Dequeue() ;
      yield return new WaitForSeconds(entry.Delay) ;

      GameEventContext entryCtx = new GameEventContextBuilder(gameObject)
        .AddValue(entry.Prefab)
        .AddValue(enemySpawnPoints[entry.SpawnPoint % enemySpawnPoints.Count].transform.position)
        .Build() ;

      GameEventSystem.SpawnEnemy.Invoke( entryCtx ) ;
    }
    yield return null ;
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

    syncPlayerScore.OnChange += OnScoreChange ;
    gameState.OnChange       += OnGameStateChange ;

    if( TimeManager != null )
    {
      TimeManager.OnTick += OnTick ;
    }

    syncPlayerScore.Value = 0 ;
    gameState.Value = GameState.WaitingForPlayers ;
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage) => false ;
#endregion
}

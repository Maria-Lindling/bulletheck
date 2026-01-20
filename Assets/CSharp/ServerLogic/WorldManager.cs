using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System;
using System.Collections;
using FishNet.Connection;

// misc
// TODO: add "retry level" button
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

  [Header("Menus")]
  [SerializeField] private GameObject gameFinishMenuPrefab ;

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


#region 
  private GameFinishMenuOverlay _gameFinishMenu ;
  private Queue<EncounterScriptableObject> _encountersQueue ;
  private readonly List<ClientShell> _connectedPlayers = new() ;
#endregion


#region
  public static PlayerSelect LocalPlayer { get ; private set; } = PlayerSelect.None ;
  public ForceSelection Force => ForceSelection.None ;
#endregion

  /// <summary>
  /// <para>If the game is waiting on players, then check if enough players are connected; otherwise return.</para>
  /// <para>If not enough players connected, display waiting message.</para>
  /// <para>Otherwise spawn player vessel and begin game.</para>
  /// </summary>
  private void CheckScenarioBegin()
  {
    if( gameState.Value != GameState.WaitingForPlayers )
      return ;

    if( _connectedPlayers.Count < 2 )
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
  /// <summary>
  /// Adds the client that connected to the list of connected clients and assigns
  /// it a seat (pilot/gunner). Then check if the scenario has enough players to
  /// begin.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnClientConnect(GameEventContext ctx)
  {
    if( ctx.Source.TryGetComponent<ClientShell>(out ClientShell clientShell))
    {
      _connectedPlayers.Add( clientShell ) ;
      clientShell.AssignSeat( (PlayerSelect) _connectedPlayers.Count ) ;
      GameEventSystem.NewCarousselMessage.Invoke( $"Player \"{clientShell.Seat}\" joined." ) ;
      SetLocalPlayer(clientShell.Owner,clientShell.Seat) ;
      ctx.Source.GetComponent<NetworkObject>().SetParent( clientLogicObject.GetComponent<NetworkObject>() ) ;

      CheckScenarioBegin() ;
    }
  }
  [TargetRpc]
  private void SetLocalPlayer(NetworkConnection conn, PlayerSelect playerSelect)
  {
    LocalPlayer = playerSelect ;
  }

  /// <summary>
  /// When the player vessel is spawned, assign it to each of the controlling
  /// players and set the gameState to 'Playing'.
  /// </summary>
  /// <param name="ctx"></param>
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

  /// <summary>
  /// If the player vessel is destroyed and it is defeated (value = true), then
  /// display death message, but do not pause the game.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnVesselDespawned(GameEventContext ctx)
  {
    if( ctx.TryReadValue<bool>(out bool value) && value == true )
    {
      GameEventContextBuilder gCtxB = new( gameObject ) ;
      gCtxB.AddValue<string>( "Bad luck!\n\nRestart game to play again!" ) ;
      GameEventSystem.SetMessage.Invoke( gCtxB.Build() ) ;
      GameEventSystem.ShowMessage.Invoke( new(gameObject) ) ;
    }
  }

  /// <summary>
  /// When the scenario begins, start spawning enemy vessels as described in
  /// the encounter queue.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnScenarioBegin(GameEventContext ctx)
  {
    _encountersQueue = new ( encounters ) ;

    gameState.Value = GameState.Playing ;

    StartCoroutine( RollOutEncounters() ) ;
  }

  /// <summary>
  /// When the encounter is finished, open the game finish menu.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnEncounterEnd(GameEventContext ctx)
  {
    gameState.Value = GameState.Finished ;
    _gameFinishMenu.OnEncounterEnd( ctx ) ;
  }

  public void OnScoreSubmitted(GameEventContext ctx)
  {
    _gameFinishMenu.OnScoreSubmitted( ctx ) ;
  }

  /// <summary>
  /// On pause menu input (ESC) pause/unpause the game. No actual pause menu
  /// exists.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnPauseMenu(GameEventContext ctx)
  {
    // DEBUG: cheat victory
    //if( gameState.Value == GameState.Playing )
    //{
    //  GameEventSystem.EncounterEnd.Invoke( new GameEventContextBuilder( gameObject ).AddValue<int>(1).Build() ) ;
    //}

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

  /// <summary>
  /// When the player/s take damage, substract the set damagePenalty from the
  /// players' score.
  /// <br/>Only do this if the game is currently playing.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnPlayerTakeDamage( GameEventContext ctx )
  {
    if( gameState.Value != GameState.Playing )
      return ;

    syncPlayerScore.Value = Math.Max( syncPlayerScore.Value - damagePenalty, 0 ) ;
  }

  /// <summary>
  /// When an enemy is defeated, add the enemy's score value to the players' score.
  /// <br/>Only do this if the game is currently playing.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnEnemyDefeated( GameEventContext ctx )
  {
    if( gameState.Value != GameState.Playing )
      return ;

    if( ctx.TryReadValue<int>(out int value) )
    {
      syncPlayerScore.Value += value ;
    }
  }

  public void OnPlayerDefeated( GameEventContext ctx )
  {
    if( gameState.Value != GameState.Playing )
      return ;

    gameState.Value = GameState.Finished ;
  }
#endregion


#region 
  /// <summary>
  /// When the players' score changes, invoke the ScorePoint event to inform
  /// the overlay to update its displayed value.
  /// </summary>
  /// <param name="prev"></param>
  /// <param name="next"></param>
  /// <param name="isServer"></param>
  private void OnScoreChange(int prev, int next, bool isServer)
  {
    GameEventSystem.ScorePoint.Invoke(
      new GameEventContextBuilder( gameObject )
        .AddValue<int>( next )
        .Build()
    ) ;
  }

  /// <summary>
  /// When the game state changes, currently we simply pause/unpause the game
  /// via timeScale.
  /// </summary>
  /// <param name="prev"></param>
  /// <param name="next"></param>
  /// <param name="isServer"></param>
  private void OnGameStateChange(GameState prev, GameState next, bool isServer)
  {
    if( isServer )
      Debug.Log( $"new game state is: {next}" ) ;
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

  /// <summary>
  /// When a switch in input mode is being requested, do so for all active players.
  /// </summary>
  /// <param name="ctx"></param>
  public void OnSwitchInputMode(GameEventContext ctx)
  {
    foreach(ClientShell player in _connectedPlayers)
    {
      player.SwitchInputMode() ;
    }
  }
#endregion


#region Encounter
  /// <summary>
  /// Create a queue of encounters from the editor configuration and set them to
  /// spawn in sequence.
  /// </summary>
  /// <returns></returns>
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

  /// <summary>
  /// Requests the spawning of enemy vessels according to the encounter until no
  /// further instructions remain in the queue.
  /// </summary>
  /// <param name="encounterQueue"></param>
  /// <returns></returns>
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
  /// <summary>
  /// slowly scrolls the backdrop downward
  /// </summary>
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

    GameObject gameFinishMenu = Instantiate( gameFinishMenuPrefab ) ;
    Spawn( gameFinishMenu ) ;
    _gameFinishMenu = gameFinishMenu.GetComponent<GameFinishMenuOverlay>() ;
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage) => false ;
#endregion
}

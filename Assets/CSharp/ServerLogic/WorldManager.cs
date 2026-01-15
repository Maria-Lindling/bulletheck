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

// TODO: look up what object pool is
// TODO: add a victory condition and victory screen
// TODO: add mechanics for accumulating score
// TODO: add "retry level" button
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
  [SerializeField] private EncounterScriptableObject encounter ;

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
#endregion


#region db
  private GameDatabaseClient _gameDatabaseClient ; 
#endregion


#region 
  private List<ClientShell> _connectedPlayers = new() ;
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
    Queue<EncounterEntry> encounterQueue = new(encounter.Data) ;

    StartCoroutine( RollOutEncounter(encounterQueue) ) ;
  }

  public void OnPauseMenu(GameEventContext ctx)
  {
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
#endregion


#region Encounter
  private IEnumerator RollOutEncounter(Queue<EncounterEntry> encounterQueue)
  {
    while( encounterQueue.Count > 0 )
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

    if( TimeManager != null )
    {
      TimeManager.OnTick += OnTick ;
    }

    gameState.Value = GameState.WaitingForPlayers ;
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage) => false ;
#endregion
}

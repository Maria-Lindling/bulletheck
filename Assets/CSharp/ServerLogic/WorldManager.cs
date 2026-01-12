using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using System;
using System.Collections;

[RequireComponent(typeof(GameDatabaseClient))]
public class WorldManager : NetworkBehaviour, IEntityController
{
#region UnityEditor
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
  private List<PlayerController> _registeredPlayers = new() ;
#endregion

#region 
  public ForceSelection Force => ForceSelection.None ;
#endregion


  private void CheckScenarioBegin()
  {
    if( _registeredPlayers.Count < 1 )
      return ;

    sceneBackdrop.SetActive( true ) ;
    gameState.Value = GameState.Playing ;
    GameEventSystem.ScenarioBegin.Invoke( new GameEventContext( gameObject ) ) ;
  }


  #region Events
  public void OnPlayerRegister(GameEventContext ctx)
  {
    if( ctx.Source.TryGetComponent<PlayerController>(out PlayerController playerController))
    {
      _registeredPlayers.Add( playerController ) ;
    }

    CheckScenarioBegin() ;
  }

  public void OnScenarioBegin(GameEventContext ctx)
  {
    Queue<EncounterEntry> encounterQueue = new(encounter.Data) ;

    StartCoroutine( RollOutEncounter(encounterQueue) ) ;
  }

  public void OnPauseMenu(GameEventContext ctx)
  {
    //testPattern.Spawn(testPatternSpawnPoint,gameObject) ;

    /**
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
    */
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
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage) => false ;
#endregion
}

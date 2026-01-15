using UnityEngine;
using FishNet.Object;
using UnityEngine.Events;
using System;
using static UnityEngine.InputSystem.InputAction;

public class GameEventSystem : NetworkBehaviour
{
#region UnityEditor
  [Header("Events")]
  [SerializeField] private GameEventsConfigPlayer playerEvents ;
  [SerializeField] private GameEventsConfigMeta metaEvents ;
#endregion

  public static UnityEvent<GameEventContext> ScorePoint { get; private set ; }
  public static UnityEvent<GameEventContext> PlayerRegister { get; private set ; }

  public static UnityEvent<GameEventContext> SpawnVessel { get; private set ; }
  public static UnityEvent<GameEventContext> VesselSpawned { get; private set ; }
  public static UnityEvent<GameEventContext> VesselDespawned { get; private set ; }

  public static UnityEvent<GameEventContext> SpawnBullet { get; private set ; }
  public static UnityEvent<GameEventContext> SpawnEnemy { get; private set ; }

  public static UnityEvent<GameEventContext> ClientConnect { get; private set ; }
  public static UnityEvent<GameEventContext> ClientDisconnect { get; private set ; }

  public static UnityEvent<GameEventContext> PauseMenu { get; private set ; }
  public static UnityEvent<GameEventContext> ScenarioBegin { get; private set ; }

  public static UnityEvent<GameEventContext> HideMessage { get; private set ; }
  public static UnityEvent<GameEventContext> ClearMessage { get; private set ; }
  public static UnityEvent<GameEventContext> SetMessage { get; private set ; }
  public static UnityEvent<GameEventContext> ShowMessage { get; private set ; }

  private void Start()
  {
    if( !IsServerInitialized )
      return ;
    
    ScorePoint      = playerEvents.scorePoint ;
    PlayerRegister  = playerEvents.playerRegister ;
    SpawnVessel     = playerEvents.spawnVessel ;
    VesselSpawned   = playerEvents.vesselSpawned ;
    VesselDespawned = playerEvents.vesselDespawned ;
    SpawnBullet     = playerEvents.spawnBullet ;
    
    ClientConnect     = metaEvents.clientConnect ;
    ClientDisconnect  = metaEvents.clientDisconnect ;
    SpawnEnemy        = metaEvents.spawnEnemy ;
    PauseMenu         = metaEvents.pauseMenu ;
    ScenarioBegin     = metaEvents.scenarioBegin ;
    HideMessage       = metaEvents.hideMessage ;
    ClearMessage      = metaEvents.clearMessage ;
    SetMessage        = metaEvents.setMessage ;
    ShowMessage       = metaEvents.showMessage ;
  }
}

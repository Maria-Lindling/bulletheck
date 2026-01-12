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
  public static UnityEvent<GameEventContext> SpawnBullet { get; private set ; }
  public static UnityEvent<GameEventContext> SpawnEnemy { get; private set ; }
  public static UnityEvent<GameEventContext> PauseMenu { get; private set ; }

  public static UnityEvent<GameEventContext> ScenarioBegin { get; private set ; }

  private void Start()
  {
    if( !IsServerInitialized )
      return ;
    
    ScorePoint = playerEvents.scorePoint ;
    PlayerRegister = playerEvents.playerRegister ;
    SpawnBullet = playerEvents.spawnBullet ;
    SpawnEnemy = metaEvents.spawnEnemy ;
    PauseMenu = metaEvents.pauseMenu ;
    ScenarioBegin = metaEvents.scenarioBegin ;
  }
}

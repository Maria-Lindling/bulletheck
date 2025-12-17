using UnityEngine;
using FishNet.Object;
using UnityEngine.Events;

public class GameEventSystem : NetworkBehaviour
{
#region UnityEditor
  [Header("Events")]
  [SerializeField] private UnityEvent<GameEventContext> onScorePoint ;
  [SerializeField] private UnityEvent<GameEventContext> onPlayerRegister ;
  [SerializeField] private UnityEvent<GameEventContext> onSpawnBall ;
#endregion

  public static UnityEvent<GameEventContext> OnScorePoint { get; private set ; }
  public static UnityEvent<GameEventContext> OnPlayerRegister { get; private set ; }
  public static UnityEvent<GameEventContext> OnSpawnBullet { get; private set ; }

  private void Start()
  {
    if( !IsServerInitialized )
      return ;
    
    OnScorePoint = onScorePoint ;
    OnPlayerRegister = onPlayerRegister ;
    OnSpawnBullet = onSpawnBall ;
  }
}

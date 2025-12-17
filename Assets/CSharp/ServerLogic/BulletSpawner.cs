using UnityEngine;
using FishNet.Object;
using System.Collections;

public class BulletSpawner : NetworkBehaviour
{
  [SerializeField] private GameObject ballPrefab ;

  public void OnSpawnBall(GameEventContext ctx)
  {
    StartCoroutine( SpawnBall( ctx.ReadValue<float>() ) ) ;
  }

  [Server]
  public IEnumerator SpawnBall(float delay)
  {
    yield return new WaitForSeconds(delay) ;
    // if( WorldManager.CurrentState == GameState.Playing )
    // {
    //   GameObject ballInstance = Instantiate(ballPrefab) ;
    //   Spawn(ballInstance) ;
    // }
  }

  private void Start()
  {
    if( !IsServerInitialized )
    {
      Destroy( gameObject ) ;
      return ;
    }
  }
}
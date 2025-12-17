using UnityEngine;
using FishNet.Object;
using System.Collections;

public class BulletSpawner : NetworkBehaviour
{
  [SerializeField] private GameObject bulletPrefab ;

  public void OnSpawnBullet(GameEventContext ctx)
  {
    StartCoroutine( SpawnBullet( ctx.ReadValue<float>() ) ) ;
  }

  [Server]
  public IEnumerator SpawnBullet(float delay)
  {
    yield return new WaitForSeconds(delay) ;
    // if( WorldManager.CurrentState == GameState.Playing )
    // {
    //   GameObject bulletInstance = Instantiate(bulletPrefab) ;
    //   Spawn(bulletInstance) ;
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
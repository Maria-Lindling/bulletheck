using UnityEngine;
using FishNet.Object;
using System.Collections;

public class BulletSpawner : NetworkBehaviour
{
  public void OnSpawnBullet(GameEventContext ctx)
  {
    StartCoroutine( SpawnBullet(
      ctx.Source,
      ctx.ReadValue<GameObject>(0),
      ctx.ReadValue<Vector3>(0),
      ctx.ReadValue<Vector3>(1),
      ctx.ReadValue<float>(0),
      ctx.ReadValue<float>(1),
      ctx.ReadValue<float>(2)
    ) ) ;
  }

  [Server]
  public IEnumerator SpawnBullet(
    GameObject source,
    GameObject prefab,
    Vector3 origin,
    Vector3 heading,
    float speed,
    float delay,
    float despawnTime
  )
  {
    yield return new WaitForSeconds(delay) ;

    GameObject bulletInstance = Instantiate( prefab, origin, Quaternion.LookRotation(heading,Vector3.up) ) ;

    bulletInstance.GetComponent<BulletController>().Initialize( source, speed ) ;
    
    Spawn(bulletInstance) ;

    StartCoroutine( DespawnBullet( bulletInstance, despawnTime ) ) ;
  }

  private IEnumerator DespawnBullet(GameObject bulletInstance, float delay)
  {
    yield return new WaitForSeconds(delay) ;

    if( bulletInstance == null )
      yield break ;

    Despawn( bulletInstance ) ;
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
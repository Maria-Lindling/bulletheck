using UnityEngine;
using FishNet.Object;
using System.Collections;

public class BulletSpawner : NetworkBehaviour
{
  [SerializeField] private GameObject bulletPrefab ;

  public void OnSpawnBullet(GameEventContext ctx)
  {
    StartCoroutine( SpawnBullet(
      ctx.ReadValue<Vector3>(0),
      ctx.ReadValue<Vector3>(1),
      ctx.ReadValue<float>(0),
      ctx.ReadValue<float>(1),
      ctx.ReadValue<float>(2),
      ctx.Source
    ) ) ;
  }

  [Server]
  public IEnumerator SpawnBullet(Vector3 origin, Vector3 heading, float delay, float speed, float despawnTime, GameObject prefab = default)
  {
    if( prefab == default)
      prefab = bulletPrefab ;

    yield return new WaitForSeconds(delay) ;

    GameObject bulletInstance = Instantiate( prefab, origin, Quaternion.LookRotation(heading,Vector3.up) ) ;

    bulletInstance.GetComponent<BulletController>().Initialize( speed ) ;
    
    Spawn(bulletInstance) ;

    StartCoroutine( DespawnBullet( bulletInstance, despawnTime ) ) ;
  }

  private IEnumerator DespawnBullet(GameObject bulletInstance, float delay)
  {
    yield return new WaitForSeconds(delay) ;

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
using UnityEngine;
using FishNet.Object;
using System.Collections;

public class BulletSpawner : NetworkBehaviour
{
  [SerializeField] private GameObject bulletPrefab ;

  public void OnSpawnBullet(GameEventContext ctx)
  {
    StartCoroutine( SpawnBullet( ctx.ReadValue<Vector3>(0), ctx.ReadValue<Vector3>(1), ctx.ReadValue<float>() ) ) ;
  }

  [Server]
  public IEnumerator SpawnBullet(Vector3 origin, Vector3 heading, float delay)
  {
    yield return new WaitForSeconds(delay) ;
    
    Debug.Log($"Heading: {heading}") ;
    GameObject bulletInstance = Instantiate( bulletPrefab, origin, Quaternion.LookRotation(heading,Vector3.up) ) ;
    
    Spawn(bulletInstance) ;
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
using UnityEngine;
using FishNet.Object;
using System.Collections;
using FishNet;

public class EnemySpawner : NetworkBehaviour
{
  [SerializeField] private GameObject gameEntitiesObject ;
  [SerializeField] float despawnTime = 30.0f ;

  public void OnSpawnEnemy(GameEventContext ctx)
  {
    StartCoroutine( SpawnEnemy( ctx.ReadValue<GameObject>(0), ctx.ReadValue<Vector3>(0) ) ) ;
  }

  [Server]
  public IEnumerator SpawnEnemy( GameObject prefab, Vector3 origin )
  {
    yield return null ;

    NetworkObject enemyInstance = NetworkManager.GetPooledInstantiated( prefab, gameEntitiesObject.transform, true ) ;

    enemyInstance.transform.position = origin ;
    
    Spawn(enemyInstance) ;

    enemyInstance.GetComponent<EnemyController>().ReInitialize() ;

    StartCoroutine( DespawnEnemy( enemyInstance, despawnTime ) ) ;
  }

  private IEnumerator DespawnEnemy(NetworkObject enemyInstance, float delay)
  {
    yield return new WaitForSeconds(delay) ;

    if( !enemyInstance.enabled )
      yield break ;

    Despawn( enemyInstance, DespawnType.Pool ) ;
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
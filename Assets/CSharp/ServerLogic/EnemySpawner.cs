using UnityEngine;
using FishNet.Object;
using System.Collections;

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

    GameObject enemyInstance = Instantiate( prefab, origin, new Quaternion() ) ;

    enemyInstance.transform.SetParent( gameEntitiesObject.transform ) ;
    
    Spawn(enemyInstance) ;

    StartCoroutine( DespawnEnemy( enemyInstance, despawnTime ) ) ;
  }

  private IEnumerator DespawnEnemy(GameObject enemyInstance, float delay)
  {
    yield return new WaitForSeconds(delay) ;

    if( enemyInstance == null )
      yield break ;

    Despawn( enemyInstance ) ;
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
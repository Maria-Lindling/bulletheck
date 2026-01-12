using UnityEngine;
using FishNet.Object;
using System.Collections;

public class VesselSpawner : NetworkBehaviour
{
  [SerializeField] private GameObject gameEntitiesObject ;

  public void OnSpawnVessel(GameEventContext ctx)
  {
    StartCoroutine( SpawnVessel( ctx.ReadValue<GameObject>(0), ctx.ReadValue<Vector3>(0) ) ) ;
  }

  [Server]
  public IEnumerator SpawnVessel( GameObject prefab, Vector3 origin )
  {
    yield return null ;

    GameObject vesselInstance = Instantiate( prefab, origin, new Quaternion() ) ;

    vesselInstance.transform.SetParent( gameEntitiesObject.transform ) ;
    
    Spawn(vesselInstance) ;

    GameEventSystem.VesselSpawned.Invoke( new GameEventContext(vesselInstance) ) ;
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
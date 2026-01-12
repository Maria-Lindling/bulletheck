using UnityEngine;
using FishNet.Object;
using System.Collections;
using System.Diagnostics;
using FishNet.Demo.AdditiveScenes;
using GameKit.Dependencies.Utilities;

public class BulletController : NetworkBehaviour, IEntityController
{
  [SerializeField] GameObject sprite ;
  [SerializeField] float damage ;

  private Rigidbody rb ;
  private GameObject _source ;
  private Vector3 lastVelocity ;
  private Vector3 goalVelocity ;
  private ForceSelection _force ;

#region Setup
  public float Speed { get ; private set ; }
#endregion


#region
  public ForceSelection Force => _force ;
#endregion


#region Events
  private void OnCollisionEnter(Collision other)
  {
    
  }

  private void OnTriggerEnter(Collider other)
  {
    if( other.gameObject.CompareTag( "Hitbox" ) )
    {
      if(
        other.transform.parent.TryGetComponent<IEntityController>(out IEntityController otherController) &&
        Force != otherController.Force
      )
      {
        // impact/damage logic
        otherController.TryDamageEntity( damage ) ;

        Speed = 0.0f ;
        StartCoroutine( ShrinkAndDespawn() ) ;
      }
    }
  }
#endregion


#region Coroutines
  private IEnumerator ShrinkAndDespawn()
  {
    rb.detectCollisions = false ;

    WaitForEndOfFrame wait = new() ;

    for( int i = 0; i < 4 ; i++ )
    {
      yield return wait ;
      transform.localScale = Vector3.one * (1.00f + i * 0.05f) ;
    }

    for( int i = 0; i < 12 ; i++ )
    {
      yield return wait ;
      transform.localScale = Vector3.one * (1.20f - i * 0.05f) ;
    }

    Despawn( DespawnType.Destroy ) ;
    Destroy( gameObject ) ;
  }

#endregion


#region Init
  public void Initialize(GameObject source, float speed)
  {
    _source = source ;
    Speed = speed ;
    _force = _source.GetComponent<IEntityController>().Force ;
  }
#endregion 


#region MonoBehavior
  private void Start()
  {
    //GetComponentInChildren<MeshRenderer>().material.color = OwnNetworkGameManager.BallColor ;
    if( !IsServerInitialized )
    {
      Destroy( this ) ;
      return ;
    }

    rb = GetComponentInChildren<Rigidbody>() ;

    goalVelocity = transform.forward ;

    sprite.transform.LookAt( transform.position + Camera.main.transform.forward ) ;
  }

  private void FixedUpdate()
  {
    lastVelocity = rb.linearVelocity ;
    rb.linearVelocity = goalVelocity.normalized * Speed ;
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage) => false ;
#endregion
}

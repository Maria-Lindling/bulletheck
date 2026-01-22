using UnityEngine;
using FishNet.Object;
using System.Collections;

public class BulletController : NetworkBehaviour, IEntityController
{
  [SerializeField] GameObject sprite ;
  [SerializeField] float damage ;

  private Rigidbody rb ;
  private GameObject _source ;
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
    if( !IsServerInitialized )
      return ;
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
    TimeManager.OnTick -= OnTick ;

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

    Despawn( DespawnType.Pool ) ;
  }

#endregion


#region Init
  public void Initialize(GameObject source, float speed)
  {
    if( !IsServerInitialized )
      return ;

    _source = source ;
    Speed = speed ;

    if( _source == null )
    {
      _force = ForceSelection.None ;
      return ;
    }
    _force = _source.GetComponent<IEntityController>().Force ;

    goalVelocity = transform.forward ;

    RealignSprite() ;

    TimeManager.OnTick += OnTick ;
  }

  [ObserversRpc]
  private void RealignSprite()
  {
    sprite.transform.LookAt( sprite.transform.position + Camera.main.transform.forward ) ;
  }

  public void ResetRigidBody()
  {
    if( rb == null )
      return ;
    
    rb.linearVelocity = Vector3.zero ;
    rb.angularVelocity = Vector3.zero ;
  }
  #endregion


  #region MonoBehavior
  private void Start()
  {
    if( !IsServerInitialized )
      return ;

    rb = GetComponentInChildren<Rigidbody>() ;
  }

  private void OnTick()
  {
    rb.linearVelocity = goalVelocity.normalized * Speed ;
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage) => false ;
#endregion
}

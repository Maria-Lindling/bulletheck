using UnityEngine;
using FishNet.Object;
using System.Collections;
using System.Diagnostics;
using FishNet.Demo.AdditiveScenes;

public class BulletController : NetworkBehaviour
{
  private Rigidbody rb ;
  private GameObject _source ;

  private Vector3 lastVelocity ;
  private Vector3 goalVelocity ;

#region Setup
  public float Speed { get ; private set ; }
#endregion

#region Events
  private void OnCollisionEnter(Collision other)
  {
    if( other.gameObject.CompareTag( "Hitbox" ) )
    {
      if(
        _source.TryGetComponent<PlayerController>(out PlayerController sourceController) &&
        _source.TryGetComponent<PlayerController>(out PlayerController myController) &&
        sourceController != myController
      )
      {
        // impact/damage logic
        StartCoroutine( ShrinkAndDespawn() ) ;
      }
    }

    //ContactPoint cp = other.contacts[0] ;
    //goalVelocity = Vector3.Reflect(lastVelocity,cp.normal) ;
    // Debug.Log("LV" + lastVelocity) ;
  }

  private void OnTriggerEnter(Collider other)
  {
    switch( other.tag )
    {
      case "LeftGoal":
        GameEventSystem.ScorePoint.Invoke( new GameEventContextBuilder(gameObject).AddValue<int>(1).Build() ) ;
        break ;
      case "RightGoal":
        GameEventSystem.ScorePoint.Invoke( new GameEventContextBuilder(gameObject).AddValue<int>(0).Build() ) ;
        break ;
      default:
        return ;
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
  }

  private void FixedUpdate()
  {
    lastVelocity = rb.linearVelocity ;
    rb.linearVelocity = goalVelocity.normalized * Speed ;
  }
#endregion
}

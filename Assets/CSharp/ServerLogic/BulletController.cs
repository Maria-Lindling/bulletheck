using UnityEngine;
using FishNet.Object;

public class BulletController : NetworkBehaviour
{
  private Rigidbody rb ;
  private float roundTime ;

  private Vector3 lastVelocity ;
  private Vector3 goalVelocity ;


#region Events
  private void OnCollisionEnter(Collision other)
  {
    ContactPoint cp = other.contacts[0] ;
    goalVelocity = Vector3.Reflect(lastVelocity,cp.normal) ;
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
    Despawn( DespawnType.Destroy ) ;
    Destroy( gameObject ) ;
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
    roundTime += Time.fixedDeltaTime ;
    lastVelocity = rb.linearVelocity ;
    rb.linearVelocity = goalVelocity.normalized * 3.667f ;
  }
#endregion
}

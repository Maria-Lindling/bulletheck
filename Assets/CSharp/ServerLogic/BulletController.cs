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
    Debug.Log("LV" + lastVelocity) ;
  }
  private void OnTriggerEnter(Collider other)
  {
    switch( other.tag )
    {
      case "LeftGoal":
        GameEventSystem.ScorePoint.Invoke( new GameEventContext(gameObject,1) ) ;
        break ;
      case "RightGoal":
        GameEventSystem.ScorePoint.Invoke( new GameEventContext(gameObject,0) ) ;
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
    goalVelocity = new Vector3( Random.Range(-1.0f, 1.0f), Random.Range(-0.5f,0.5f),0) ;
  }

  private void FixedUpdate()
  {
    roundTime += Time.fixedDeltaTime ;
    lastVelocity = rb.linearVelocity ;
    rb.linearVelocity = goalVelocity.normalized * Mathf.Max(roundTime / 10.0f, 4.0f) ;
  }
#endregion
}

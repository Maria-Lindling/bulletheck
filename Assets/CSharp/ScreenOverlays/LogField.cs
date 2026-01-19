using UnityEngine;
using FishNet.Object;
using System.Collections;
using Unity.VisualScripting;

public class LogField : NetworkBehaviour
{

  public float SlideInTime { get ; private set ; }
  public float LingerTime { get ; private set ; }
  public float SlideUpTime { get ; private set ; }

  public bool IsOnScreen { get; private set ; } = false ;
  public bool Lingering { get; private set ; } = false ;

  public LogField PrevSibling { get ; private set ; }
  public LogField NextSibling { get ; private set ; }

  public Vector3 PrevPosition { get ; private set ; } = new Vector3( -960.0f, -120.0f, 0.0f ) ;
  public Vector3 InitialPosition { get ; private set ; } = new Vector3( -960.0f, -120.0f, 0.0f ) ;
  public Vector3 NextPosition { get ; private set ; } = new Vector3( 0.0f, -120.0f, 0.0f ) ;



  public void SlideAndLinger( LogField precedingSibling )
  {
    StartCoroutine( SlideIn() ) ;
  }

  private IEnumerator SlideIn()
  {
    PrevPosition = InitialPosition ;
    transform.localPosition = PrevPosition ;
    Vector3 endPosition = NextPosition ;
    
    if( PrevSibling.Lingering )
    {
      endPosition             += new Vector3( 0.0f, -120 , 0.0f );
      transform.localPosition += new Vector3( 0.0f, -120 , 0.0f );
    }
    Lingering = true ;

    int totalFrames = (int) (SlideInTime / Time.fixedDeltaTime) ;
    Vector3 prev = transform.localPosition ;
    Vector3 increment = ( endPosition - transform.localPosition ) / totalFrames ;
    Vector3 intermediary ;
    Vector3 next ;

    for( int i = 0; i < totalFrames ; i++ )
    {
      intermediary = prev ;
      next = intermediary + increment ;
      prev = next ;
      yield return new WaitForEndOfFrame() ;
      transform.localPosition = next ;
    }
    
    if( PrevSibling.IsOnScreen )
      yield return new WaitUntil( () => !PrevSibling.Lingering ) ;

    PrevPosition = endPosition ;
    NextPosition = PrevSibling.PrevPosition ;
    
    yield return StartCoroutine( SlideUp() ) ;

    if( IsOnScreen )
      yield return StartCoroutine( SlideUp() ) ;
  }



  private IEnumerator SlideUp()
  {
    Vector3 endPosition = NextPosition ;

    int totalFrames = (int) (SlideUpTime / Time.fixedDeltaTime) ;
    Vector3 prev = transform.localPosition ;
    Vector3 increment = ( endPosition - transform.localPosition ) / totalFrames ;
    Vector3 intermediary ;
    Vector3 next ;

    for( int i = 0; i < totalFrames ; i++ )
    {
      intermediary = prev ;
      next = intermediary + increment ;
      prev = next ;
      yield return new WaitForEndOfFrame() ;
      transform.localPosition = next ;
    }
  }

  private void Awake() { }

  private void Update() { }
}

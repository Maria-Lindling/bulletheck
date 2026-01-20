using UnityEngine;
using FishNet.Object;
using System.Collections;
using Unity.VisualScripting;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine.Rendering;

public class LogField : NetworkBehaviour
{

  private TextMeshProUGUI textField ;
  private float width ;
  private float height ;

  public float SlideInTime { get ; private set ; }
  public float LingerTime { get ; private set ; }
  public float SlideUpTime { get ; private set ; }

  public bool IsOnScreen => rt != null && rt.localPosition.x > -width && rt.localPosition.y < 0.0f;
  
  public Vector3 PrevPosition { get ; private set ; }
  public Vector3 InitialPosition { get ; private set ; }
  public Vector3 NextPosition { get ; private set ; }

  private RectTransform rt ;

  private readonly SyncVar<string> syncText = new () ;

  [Server]
  public void SetSlideTimes( float timeIn, float timeUp )
  {
    SlideInTime = timeIn ;
    SlideUpTime = timeUp ;
  }

  [Server]
  public void SetLingerTime( float value )
  {
    LingerTime = value ;
  }

  [Server]
  public void SetText( string value )
  {
    syncText.Value = value ;
  }

  private void OnTextChange( string prev, string next, bool isServer )
  {
    textField.text = next ;
  }


  [Server]
  public void SlideAndLinger( int value )
  {
    StartCoroutine( SlideIn( value ) ) ;
  }

  [Server]
  private IEnumerator SlideIn( int value )
  {
    PrevPosition        = InitialPosition ;
    rt.localPosition    = PrevPosition ;
    Vector3 endPosition = NextPosition ;
    
    if( value > 0 )
    {
      endPosition      += new Vector3( 0.0f, -height * value, 0.0f );
      rt.localPosition += new Vector3( 0.0f, -height * value, 0.0f );
    }


    int totalFrames = (int) (SlideInTime / Time.fixedDeltaTime) ;
    Vector3 prev = rt.localPosition ;
    Vector3 increment = ( endPosition - rt.localPosition ) / totalFrames ;
    Vector3 intermediary ;
    Vector3 next ;

    //Debug.Log( $"Moving message from InitialPosition {InitialPosition} to endPosition {endPosition} over {totalFrames} frames." ) ;

    for( int i = 0; i < totalFrames ; i++ )
    {
      intermediary = prev ;
      next = intermediary + increment ;
      prev = next ;
      yield return new WaitForEndOfFrame() ;
      rt.localPosition = next ;
    }

    while( IsOnScreen )
    {
      NextPosition = rt.localPosition + new Vector3( 0.0f, height, 0.0f ) ;
      yield return new WaitForSeconds( LingerTime ) ;
      yield return StartCoroutine( SlideUp() ) ;
    }
  }


  [Server]
  private IEnumerator SlideUp()
  {
    Vector3 endPosition = NextPosition ;

    int totalFrames = (int) (SlideUpTime / Time.fixedDeltaTime) ;
    Vector3 prev = rt.localPosition ;
    Vector3 increment = ( endPosition - rt.localPosition ) / totalFrames ;
    Vector3 intermediary ;
    Vector3 next ;

    for( int i = 0; i < totalFrames ; i++ )
    {
      intermediary = prev ;
      next = intermediary + increment ;
      prev = next ;
      yield return new WaitForEndOfFrame() ;
      rt.localPosition = next ;
    }
  }

  private void Start()
  {
    syncText.OnChange += OnTextChange ;

    textField = GetComponentInChildren<TextMeshProUGUI>() ;

    if( !IsServerInitialized )
      return ;

    rt = GetComponent<RectTransform>() ;

    width  = Mathf.Abs( rt.sizeDelta.x ) ;
    height = Mathf.Abs( rt.sizeDelta.y ) ;

    InitialPosition = rt.localPosition ;
    PrevPosition    = InitialPosition ;
    NextPosition    = PrevPosition + new Vector3( width, 0.0f, 0.0f ) ;
  }

  private void Update() { }
}

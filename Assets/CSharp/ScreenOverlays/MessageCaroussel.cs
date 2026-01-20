using UnityEngine;
using FishNet.Object;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class MessageCaroussel : NetworkBehaviour
{

  [SerializeField] List<LogField> logFields ;

  [SerializeField] float slideInTime ;
  [SerializeField] float slideUpTime ;
  [SerializeField] float lingerTime ;

  private Queue<LogField> NextInLine { get ; } = new () ; 

  [Server]
  public void OnNewMessage( string message )
  {
    Debug.Log( message ) ;
    if( NextInLine.Count == 0 )
    {
      Debug.LogError( "Queue is empty for some reason." ) ;
    }

    LogField next = NextInLine.Dequeue() ;
    next.SetText( message ) ;
    next.SlideAndLinger( logFields.Sum( (lf) => lf.IsOnScreen ? 1 : 0 ) ) ;
    NextInLine.Enqueue( next ) ;
  }


  private void Start()
  {
    if( !IsServerInitialized )
      return ;
    
    foreach( LogField logField in logFields )
    {
      NextInLine.Enqueue( logField ) ;
      logField.SetSlideTimes( slideInTime, slideUpTime ) ;
      logField.SetLingerTime( lingerTime ) ;
    }
  }

  private void Update() { }
}

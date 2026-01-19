using UnityEngine;
using FishNet.Object;
using System.Collections.Generic;
using System.Collections;

public class ScreenLog : NetworkBehaviour
{

  [SerializeField] List<LogField> logFields ;

  private Queue<LogField> NextInLine { get ; set ; } = new () ; 

  public void OnNewMessage( string message )
  {
    
  }


  private void Awake()
  {
    foreach( LogField logField in logFields )
    {
      NextInLine.Enqueue( logField ) ;
    }
  }

  private void Update() { }
}

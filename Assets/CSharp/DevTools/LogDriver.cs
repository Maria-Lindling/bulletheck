using FishNet.Object;
using UnityEditor ;
using UnityEngine ;

public class LogDriver : NetworkBehaviour
{
  [ServerRpc]
  public void LogToServer(string origin, string message)
  {
    Debug.Log( $"[{origin}]: {message}" ) ;
  }
}
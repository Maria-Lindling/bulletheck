using UnityEngine;
using FishNet.Object;
using GameKit.Dependencies.Utilities;
using UnityEngine.UI;
using TMPro;
using FishNet.Transporting.Tugboat;
using System;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using FishNet.Managing;
using System.Net;
using FishNet.Authenticating;
using System.Net.Sockets;

public class GameSetupMenuOverlay : MonoBehaviour
{
#region static
  static Regex RegexIP = new ( @"^(?:(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])(\.(?!$)|$)){4}$" ) ; 
#endregion

#region Unity Editor
  [SerializeField] GameObject topMenu ;
  [SerializeField] MenuConfigClientOrHost hostMenuConfig ;
  [SerializeField] MenuConfigClientOrHost clientMenuConfig ;
#endregion


#region 
  private Tugboat _tugboat ;
#endregion


#region Events
  public void OnClickButtonTopMenu()
  {
    hostMenuConfig.Top.SetActive(false) ;
    clientMenuConfig.Top.SetActive(false) ;
    topMenu.SetActive(true) ;
  }

  public void OnClickButtonHostMenu()
  {
    clientMenuConfig.Top.SetActive(false) ;
    topMenu.SetActive(false) ;
    
    hostMenuConfig.Top.SetActive(true) ;

    hostMenuConfig.IP.text = GetFirstInterNetworkIP() ;
  }

  public void OnClickButtonConnectMenu()
  {
    hostMenuConfig.Top.SetActive(false) ;
    topMenu.SetActive(false) ;
    
    clientMenuConfig.Top.SetActive(true) ;
  }

  public void OnClickButtonHost()
  {
    if( !ValidatePort( hostMenuConfig.Port.text, out ushort port ) )
    {
      Debug.LogError( $"Invalid Port \"{hostMenuConfig.Port.text}\"" ) ;
      return ;
    }

    topMenu.SetActive(false) ;

    _tugboat.SetPort( port ) ;
    _tugboat.StartConnection( true ) ;

    gameObject.SetActive(false) ;
  }

  public void OnClickButtonConnect()
  {
    if( !ValidateIP( clientMenuConfig.IP.text, out string ipAddress ) )
    {
      Debug.LogError( $"Invalid IP \"{clientMenuConfig.IP.text}\"" ) ;
      return ;
    }

    if( !ValidatePort( clientMenuConfig.Port.text, out ushort port ) )
    {
      Debug.LogError( $"Invalid Port \"{clientMenuConfig.Port.text}\"" ) ;
      return ;
    }
    
    topMenu.SetActive(false) ;

    _tugboat.SetClientAddress( ipAddress ) ;
    _tugboat.SetPort( port ) ;
    _tugboat.StartConnection( false ) ;

    gameObject.SetActive(false) ;
  } 
#endregion


#region Validation
  private bool ValidateIP(string value, out string parsedValue)
  {
    Match m = RegexIP.Match(value) ;
    if( m.Success )
    {
      parsedValue = m.Value ;
      return true ;
    }

    parsedValue = string.Empty ;
    return false ;
  }

  private bool ValidatePort(string value, out ushort parsedValue)
  {
    if( ushort.TryParse( value, out parsedValue ) )
    {
      return true ;
    }
    
    parsedValue = 0 ;
    return false ;
  }
  #endregion

  private string GetFirstInterNetworkIP()
  {
    foreach( IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList )
    {
      if( ip.AddressFamily == AddressFamily.InterNetwork )
      {
        return ip.ToString() ;
      }
    }
    return string.Empty ;
  }

  private void Awake()
  {
    hostMenuConfig.Top.SetActive(false) ;
    clientMenuConfig.Top.SetActive(false) ;
    topMenu.SetActive(true) ;

    Debug.Log( $"My IP is: {GetFirstInterNetworkIP()}" ) ;
  }

  private void Start()
  {
    _tugboat = GameObject.Find("NetworkManager").GetComponent<Tugboat>() ;
  }

  private void Update() { }
}

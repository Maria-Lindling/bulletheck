using UnityEngine;
using FishNet.Transporting.Tugboat;
using System.Text.RegularExpressions;
using System.Net;
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
    hostMenuConfig.Top.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    clientMenuConfig.Top.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    topMenu.transform.localPosition = Vector3.zero ;
  }

  public void OnClickButtonHostMenu()
  {
    clientMenuConfig.Top.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    topMenu.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    
    hostMenuConfig.Top.transform.localPosition = Vector3.zero ;

    hostMenuConfig.IP.text = GetFirstInterNetworkIP() ;

    if( !ValidatePort( /*hostMenuConfig.Port.text*/ "7700", out ushort port ) )
    {
      Debug.LogError( $"Invalid Port \"{hostMenuConfig.Port.text}\"" ) ;
      return ;
    }
    _tugboat.SetPort( port ) ;
    _tugboat.StartConnection( true ) ;

    _tugboat.SetClientAddress( "localhost" ) ;
    _tugboat.StartConnection( false ) ;
  }

  public void OnClickButtonConnectMenu()
  {
    hostMenuConfig.Top.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    topMenu.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    
    clientMenuConfig.Top.transform.localPosition = Vector3.zero ;
  }

  public void OnClickButtonHost()
  {

    topMenu.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;

    gameObject.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
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
    
    topMenu.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;

    _tugboat.SetClientAddress( ipAddress ) ;
    _tugboat.SetPort( port ) ;
    _tugboat.StartConnection( false ) ;

    gameObject.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
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
    hostMenuConfig.Top.transform.localPosition   = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    clientMenuConfig.Top.transform.localPosition = new Vector3( -1920.0f, 0.0f, 0.0f ) ;
    topMenu.transform.localPosition = Vector3.zero ;

    Debug.Log( $"My IP is: {GetFirstInterNetworkIP()}" ) ;
  }

  private void Start()
  {
    _tugboat = GameObject.Find("NetworkManager").GetComponent<Tugboat>() ;
  }

  private void Update() { }
}

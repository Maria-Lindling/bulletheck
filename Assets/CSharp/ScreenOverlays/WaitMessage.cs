using System.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;


/**
  wait message doesn't appear for client when using networkhudcanvas
  because it's not a network object
  
  I will fix this later or never
*/
[RequireComponent(typeof(TextMeshProUGUI))]
public class WaitMessage : NetworkBehaviour 
{
#region sync
  private readonly SyncVar<string> syncMessage = new () ;
  private readonly SyncVar<bool> syncVisibility = new () ;
#endregion

#region 
  public void OnHideMessage(GameEventContext ctx)
  {
    syncVisibility.Value = false ;
  }

  public void OnClearMessage(GameEventContext ctx)
  {
    syncMessage.Value = string.Empty ;
  }

  public void OnSetMessage(GameEventContext ctx)
  {
    if( ctx.TryReadValue<string>(out string value) )
    {
      syncMessage.Value = value ;
    }
  }

  public void OnShowMessage(GameEventContext ctx)
  {
    syncVisibility.Value = true ;
  }

  private void OnMessageChanged(string prev, string next, bool isServer)
  {
    GetComponent<TextMeshProUGUI>().text = syncVisibility.Value ? next : string.Empty ;
  }

  private void OnVisibilityChanged(bool prev, bool next, bool isServer)
  {
    GetComponent<TextMeshProUGUI>().text = next ? syncMessage.Value : string.Empty ;
  }
#endregion


#region Init
  private void OnDisable()
  {
    syncMessage.OnChange    -= OnMessageChanged ;
    syncVisibility.OnChange -= OnVisibilityChanged ;
  }

  private IEnumerator DelayedInit()
  {
    syncMessage.OnChange    += OnMessageChanged ;
    syncVisibility.OnChange += OnVisibilityChanged ;
    
    yield return null ;
    if( IsServerInitialized )
    {
      syncMessage.Value    = string.Empty ;
      syncVisibility.Value = false ;
    }
  }
#endregion


#region MonoBehavior
  private void Start()
  {
    StartCoroutine( DelayedInit() ) ;
  }
#endregion
  
}

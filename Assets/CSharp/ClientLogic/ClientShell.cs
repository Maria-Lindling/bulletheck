using UnityEngine;
using FishNet.Object;
using System.Collections;
using UnityEngine.InputSystem;
using FishNet.Object.Synchronizing;
using FishNet.Demo.AdditiveScenes;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput))]
public class ClientShell : NetworkBehaviour
{
#region SyncVar
  private readonly SyncVar<PlayerSelect> syncPlayerSeat = new() ;
  private readonly SyncVar<PlayerVessel> syncPlayerVessel = new() ;
  private readonly SyncVar<bool> syncIsReady = new() ;
#endregion


#region 
  private PlayerInput _playerInput ;
#endregion


#region 
  [Server]
  public void AssignSeat(PlayerSelect value)
  {
    syncPlayerSeat.Value = value ;
  }
  [Server]
  public void AssignVessel(PlayerVessel value)
  {
    syncPlayerVessel.Value = value ;
  }
#endregion


#region OwnerExclusive
  /// <summary>
  /// <para>Action taken on primary attack input. (Keyboard default: mouse-1)</para>
  /// <para>Implicitly Owner-exclusive.</para>
  /// </summary>
  /// <param name="ctx"></param>
  public void OnPrimaryAttack(CallbackContext ctx)
  {
    if( syncPlayerSeat.Value == PlayerSelect.Player1 )
    {
      Atk1Rpc( ctx.ReadValueAsButton() ) ;
    }
    else if( syncPlayerSeat.Value == PlayerSelect.Player2 )
    {
      Atk3Rpc( ctx.ReadValueAsButton() ) ;
    }

    if( !ctx.performed )
      return ;
  }

  /// <summary>
  /// <para>Action taken on primary attack input. (Keyboard default: mouse-1)</para>
  /// <para>Implicitly Owner-exclusive.</para>
  /// </summary>
  /// <param name="ctx"></param>
  public void OnSecondaryAttack(CallbackContext ctx)
  {
    if( syncPlayerSeat.Value == PlayerSelect.Player1 )
    {
      // evasive maneuver ?
    }
    if( syncPlayerSeat.Value == PlayerSelect.Player2 )
    {
      Atk2Rpc( ctx.ReadValueAsButton() ) ;
    }

    if( !ctx.performed )
      return ;
  }

  /// <summary>
  /// <para>Action taken on movement input. (Keyboard default: WASD)</para>
  /// <para>Implicitly Owner-exclusive.</para>
  /// </summary>
  /// <param name="ctx">Contains movement direction as Vector2.</param>
  public void OnMove(CallbackContext ctx)
  {
    if( syncPlayerSeat.Value == PlayerSelect.Player1 )
    {
      MoveRpc( (Vector3)ctx.ReadValue<Vector2>().normalized ) ;
    }
    if( syncPlayerSeat.Value == PlayerSelect.Player2 )
    {
      // nothing, maybe directional shields?
    }

    if( !ctx.performed )
      return ;
  }

  public void OnLook(CallbackContext ctx)
  {
    if( syncPlayerSeat.Value == PlayerSelect.Player1 )
    {
      // nothing
    }
    if( syncPlayerSeat.Value == PlayerSelect.Player2 )
    {
      //LookRpc( (Vector3)Camera.main.ScreenToWorldPoint( (Vector3)ctx.ReadValue<Vector2>() ) ) ;
      LookRpc( (Vector3)ctx.ReadValue<Vector2>() ) ;
    }

    if( !ctx.performed )
      return ;
  }

  /// <summary>
  /// <para>Action taken on interact input. (Keyboard default: spacebar)</para>
  /// <para>Implicitly Owner-exclusive.</para>
  /// </summary>
  /// <param name="ctx"></param>
  public void OnInteract(CallbackContext ctx)
  {
    if( !ctx.performed )
      return ;
  }

  /// <summary>
  /// <para>Action taken on pause menu input. (Keyboard default: escape)</para>
  /// <para>Implicitly Owner-exclusive.</para>
  /// </summary>
  /// <param name="ctx"></param>
  public void OnPauseMenu(CallbackContext ctx)
  {
    if( !ctx.performed )
      return ;
    
    PauseMenu() ;
  }
#endregion


#region Invoke Server
  [ServerRpc]
  private void ClientConnected()
  {
    GameEventSystem.ClientConnect.Invoke( new GameEventContext( gameObject ) ) ;
  }

  [ServerRpc]
  private void MoveRpc(Vector3 value)
  {
    syncPlayerVessel.Value.syncMove.Value = value ;
  }

  [ServerRpc]
  private void LookRpc(Vector3 value)
  {
    syncPlayerVessel.Value.syncLook.Value = value ;
  }

  [ServerRpc]
  private void Atk1Rpc(bool value)
  {
    syncPlayerVessel.Value.syncAtk1.Value = value ;
  }

  [ServerRpc]
  private void Atk2Rpc(bool value)
  {
    syncPlayerVessel.Value.syncAtk2.Value = value ;
  }

  [ServerRpc]
  private void Atk3Rpc(bool value)
  {
    syncPlayerVessel.Value.syncAtk3.Value = value ;
  }

  [ServerRpc]
  private void PauseMenu()
  {
    GameEventSystem.PauseMenu.Invoke( new GameEventContext( gameObject ) ) ;
  }
#endregion


#region Handle Server
  private void OnSeatChange(PlayerSelect prev, PlayerSelect next, bool isServer )
  {
    
  }
  
  private void OnVesselChange(PlayerVessel prev, PlayerVessel next, bool isServer )
  {
    
  }

  private void OnReadyChange(bool prev, bool next, bool isServer )
  {
    
  }
#endregion


#region Init
  private IEnumerator DelayedIsOwner()
  {
    syncPlayerSeat.OnChange += OnSeatChange ;
    syncPlayerVessel.OnChange += OnVesselChange ;
    syncIsReady.OnChange += OnReadyChange ;

    _playerInput = GetComponent<PlayerInput>() ;
    _playerInput.enabled = false ;

    yield return null ;
    if( IsOwner )
    {
      _playerInput.enabled = true ;

      ClientConnected() ;
    }
    else if( IsServerInitialized )
    {
      
    }
  }
#endregion


#region MonoBehavior
  private void Start()
  {
    StartCoroutine( DelayedIsOwner() ) ;
  }
#endregion
}

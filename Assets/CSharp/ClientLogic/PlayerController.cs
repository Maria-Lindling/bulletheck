using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using System.Collections;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SocialPlatforms.Impl;

[RequireComponent(typeof(LogDriver))]
[RequireComponent(typeof(PlayerInput))]

// [RequireComponent(typeof(PlayerInput))]
// [RequireComponent(typeof(LogDriver))]
public class PlayerController : NetworkBehaviour
{
#region SyncVar
  private readonly SyncVar<Color> playerColor = new() ;
  private readonly SyncVar<bool> isReady = new() ;
#endregion


#region Fields
  private Renderer playerRenderer ;
  private LogDriver logDriver ;
  private PlayerInput playerInput ;
  private Vector2 _currentMovement ;
#endregion


#region Properties
  public bool IsReady => isReady.Value ;
  public PlayerSelect Identity { get ; private set ; }
#endregion


#region Unity Editor
  [SerializeField] private float moveSpeed = 5.0f ;
  [SerializeField] private float minX = -8.75f ;
  [SerializeField] private float maxX = 8.75f ;
  [SerializeField] private float minY = -4.5f ;
  [SerializeField] private float maxY = 4.75f ;
#endregion


#region ServerExclusive
#endregion


#region OwnerExclusive
  /// <summary>
  /// <para>Action taken on primary attack input. (Keyboard default: mouse-1)</para>
  /// <para>Implicitly Owner-exclusive.</para>
  /// </summary>
  /// <param name="ctx"></param>
  public void OnPrimaryAttack(CallbackContext ctx)
  {
    if( !ctx.performed )
      return ;
    
    Test_PrimaryAttack() ;
  }

  /// <summary>
  /// <para>Action taken on movement input. (Keyboard default: WASD)</para>
  /// <para>Implicitly Owner-exclusive.</para>
  /// </summary>
  /// <param name="ctx">Contains movement direction as Vector2.</param>
  public void OnMove(CallbackContext ctx)
  {
    Move( ctx.ReadValue<Vector2>() ) ;
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

    ChangeColor( Random.ColorHSV() ) ;
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


#region ServerRpc
  [ServerRpc]
  private void Test_PrimaryAttack()
  {
    GameEventSystem.ScorePoint.Invoke( new GameEventContextBuilder(gameObject).AddValue<int>(Random.Range(100,10000000)).AddValue<string>("#### #### 0000").Build() ) ;
  }

  [ServerRpc]
  private void Move(Vector2 value)
  {
    _currentMovement = moveSpeed * Time.deltaTime * value.normalized ;
  }
  
  [ServerRpc]
  private void PauseMenu()
  {
    GameEventSystem.PauseMenu.Invoke( new GameEventContext( gameObject ) ) ;
  }

  [ServerRpc]
  private void UpdatePosition()
  {
    transform.position = new Vector3(
      Mathf.Clamp( transform.position.x + _currentMovement.x, minX, maxX ),
      Mathf.Clamp( transform.position.y + _currentMovement.y, minY, maxY ),
      0
    ) ;
  }

  [ServerRpc]
  private void ChangeColor(float red, float green, float blue)
  {
    playerColor.Value = new Color(red,green,blue) ;
  }

  [ServerRpc]
  private void ChangeColor(Color value)
  {
    playerColor.Value = value ;
  }

  [ServerRpc]
  public void SetReadyStateServerRpc(string name)
  {
    // Debug.Log($"SetPlayerReady: {name}") ;
    isReady.Value = !isReady.Value ;

    GameEventSystem.PlayerRegister.Invoke( new GameEventContextBuilder(gameObject).AddValue<int>(transform.position.x < 0 ? 0 : 1).AddValue<string>(name).Build() ) ;
  }
#endregion


#region SyncEvents
  private void OnColorChanged(Color prev, Color next, bool isServer )
  {
    playerRenderer.material.color = next ;
  }
#endregion


#region Init
  private void OnDisable()
  {
    playerColor.OnChange -= OnColorChanged ;
    if( IsOwner )
    {
      playerInput.enabled = false ;
      logDriver.enabled   = false ;
      if( TimeManager != null )
      {
        TimeManager.OnTick -= OnTick ;
      }
    }
  }

  private IEnumerator DelayedIsOwner()
  {
    playerColor.OnChange += OnColorChanged ;

    playerRenderer  = GetComponentInChildren<Renderer>() ;
    playerRenderer.material = new Material(playerRenderer.material) ;
    playerRenderer.material.color = playerColor.Value ;

    playerInput     = GetComponent<PlayerInput>() ;
    logDriver       = GetComponent<LogDriver>() ;
    playerInput.enabled = false ;
    logDriver.enabled   = false ;

    Identity = transform.position.x < 0 ? PlayerSelect.Player1 : PlayerSelect.Player2 ;
    
    yield return null ;
    if( IsOwner )
    {
      switch( Identity )
      {
        case PlayerSelect.Player1 :
          ChangeColor( Color.red ) ;
          break ;

        case PlayerSelect.Player2 :
          ChangeColor( Color.blue ) ;
          break ;

        default:
          ChangeColor( Random.ColorHSV() ) ;
          break ;
      }

      playerInput.enabled = true ;
      logDriver.enabled   = true ;

      if( TimeManager != null )
      {
        TimeManager.OnTick += OnTick ;
      }
    }
    else if( IsServerInitialized )
    {
      logDriver.enabled = true ;
    }
  }
#endregion


#region OnTick
  private void OnTick()
  {
    if( !IsOwner )
      return ;

    UpdatePosition() ;
  }
#endregion


#region MonoBehavior
  private void Start()
  {
    StartCoroutine( DelayedIsOwner() ) ;
  }
#endregion
}

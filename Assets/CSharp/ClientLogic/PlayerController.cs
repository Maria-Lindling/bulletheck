using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
#region SyncVar
  private readonly SyncVar<Color> playerColor = new() ;
  private readonly SyncVar<bool> isReady = new() ;
#endregion


#region Fields
  private Renderer playerRenderer ;
#endregion


#region Properties
  public bool IsReady => isReady.Value ;
  public int PlayerNumber => transform.position.x < 0 ? 0 : 1 ;
#endregion


#region Unity Editor
  [SerializeField] private float moveSpeed = 5.0f ;
  [SerializeField] private float minY = -4.0f ;
  [SerializeField] private float maxY = 4.0f ;

  [Header("Input System")]
  [SerializeField] private InputAction moveAction ;
  [SerializeField] private InputAction colorChangeAction ;
#endregion


#region OnTick
  private void OnTick()
  {
    if( !IsOwner )
      return ;
    
    if( isReady.Value )
    {
      HandleMoveInput() ;
    }
    else
    {
      CheckForChangeColor() ;
    }
  }
#endregion


#region ReadyStateHandling
  [ServerRpc]
  public void SetReadyStateServerRpc(string name)
  {
    // Debug.Log($"SetPlayerReady: {name}") ;
    isReady.Value = !isReady.Value ;

    GameEventSystem.OnPlayerRegister.Invoke( new GameEventContext(gameObject, transform.position.x < 0 ? 0 : 1, name) ) ;
  }
#endregion


#region Movement
  private void HandleMoveInput()
  {
    float input = moveAction.ReadValue<float>() ;
    if( input != 0.0f )
      Move(input) ;
  }

  [ServerRpc]
  private void Move(float value)
  {
    float newY = transform.position.y + value * moveSpeed * Time.deltaTime ;
    newY = Mathf.Clamp( newY, minY, maxY ) ;
    transform.position = new Vector3( transform.position.x, newY, transform.position.z ) ;
  }
#endregion


#region Color
  private void CheckForChangeColor()
  {
    if( !colorChangeAction.triggered )
      return ;

    ChangeColor( Random.ColorHSV() ) ;
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
      moveAction?.Disable() ;
      colorChangeAction?.Disable() ;
      if( TimeManager != null )
      {
        TimeManager.OnTick -= OnTick ;
      }
    }
  }

  private IEnumerator DelayedIsOwner()
  {
    playerColor.OnChange += OnColorChanged ;
    playerRenderer = GetComponentInChildren<Renderer>() ;
    playerRenderer.material = new Material(playerRenderer.material) ;
    playerRenderer.material.color = playerColor.Value ;
    yield return null ;
    if( IsOwner )
    {
      switch( PlayerNumber )
      {
        case 0 :
          ChangeColor( Color.red ) ;
          break ;
        case 1 :
          ChangeColor( Color.blue ) ;
          break ;
        default:
          ChangeColor( Random.ColorHSV() ) ;
          break ;
      }

      moveAction?.Enable() ;
      colorChangeAction?.Enable() ;

      if( TimeManager != null )
      {
        TimeManager.OnTick += OnTick ;
      }
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

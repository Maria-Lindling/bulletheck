using UnityEngine;
using FishNet.Object;
using TMPro;
using FishNet.Object.Synchronizing;
using System.Collections;

public class ScoreDisplay : NetworkBehaviour
{
#region const
  private const string PLACEHOLDER_LABEL = "[display label]" ;
  private const string PLACEHOLDER_VALUE = "[display value]" ;
  private const string DEFAULT_NUMBER_PATTERN = "#### 0000 0000" ;
#endregion


#region Unity Editor
  [Header("Presets")]
  [SerializeField] private PlayerSelect linkedPlayer = PlayerSelect.None ;
  [SerializeField] private string defaultValue ;
  [SerializeField] private bool isNumber = true ;

  [Header("Text")]
  [SerializeField] private TextMeshProUGUI labelText ;
  [SerializeField] private TextMeshProUGUI valueText ;
#endregion


#region SyncVar
  private readonly SyncVar<PlayerSelect> LinkedPlayer = new() ;
  private readonly SyncVar<string> Label = new() ;
  private readonly SyncVar<string> Value = new() ;
#endregion


#region GameEvents
  public void OnValueChange(GameEventContext ctx)
  {
    if( ctx.ReadValue<PlayerSelect>() != LinkedPlayer.Value )
      return ;
    
    if( isNumber )
    {
      Value.Value = ctx.ReadValue<int>().ToString( ctx.TryReadValue(out string pattern) ? pattern : DEFAULT_NUMBER_PATTERN) ;
    }
    else
    {
      Value.Value = ctx.ReadValue<string>() ;
    }
  }
#endregion


#region Show/Hide
  public void Show()
  {
    labelText.enabled = true ;
    valueText.enabled = true ;
  }

  public void Hide()
  {
    labelText.enabled = false ;
    valueText.enabled = false ;
  }
#endregion


#region SyncEvent
  private void OnLinkedPlayerChange(PlayerSelect prev, PlayerSelect next, bool asServer)
  {
    if( next == PlayerSelect.None )
    {
      Hide() ;
      Value.Value = defaultValue ;
      return ;
    }
    else
    {
      
    }
  }

  private void OnLabelChange(string prev, string next, bool asServer)
  {
    labelText.text = next ;
  }

  private void OnValueChange(string prev, string next, bool asServer)
  {
    valueText.text = next ;
  }
#endregion


#region MonoBehavior
private void OnDisable()
  {
    LinkedPlayer.OnChange -= OnLinkedPlayerChange ;
    Label.OnChange -= OnLabelChange ;
    Value.OnChange -= OnValueChange ;
    Hide() ;
  }

  private IEnumerator DelayedInitialization()
  {
    LinkedPlayer.OnChange += OnLinkedPlayerChange ;
    Label.OnChange += OnLabelChange ;
    Value.OnChange += OnValueChange ;

    yield return null ;
    if( IsServerInitialized )
    {
      LinkedPlayer.Value = linkedPlayer ;
      Value.Value = defaultValue ;
    }
    
    Show() ;
  }

  private void Start()
  {
    Hide() ;
    StartCoroutine( DelayedInitialization() ) ;
  }
#endregion
}

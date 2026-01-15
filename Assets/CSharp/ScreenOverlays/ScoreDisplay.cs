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
  private const int FLASH_TICKS = 6 ;
  private readonly Color BASE_COLOR = Color.white ;
  private readonly Color FLASH_COLOR = Color.yellow ;
#endregion


#region Unity Editor
  [Header("Presets")]
  [SerializeField] private string defaultValue ;
  [SerializeField] private bool isNumber = true ;

  [Header("Text")]
  [SerializeField] private TextMeshProUGUI labelText ;
  [SerializeField] private TextMeshProUGUI valueText ;
#endregion


#region SyncVar
  private readonly SyncVar<string> Label = new() ;
  private readonly SyncVar<string> Value = new() ;
#endregion


#region 
  private bool _valueFlashLock = false ;
#endregion

#region GameEvents
  public void OnValueChange(GameEventContext ctx)
  {    
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


#region SyncEvent
  private void OnLabelChange(string prev, string next, bool asServer)
  {
    labelText.text = next ;
  }

  private void OnValueChange(string prev, string next, bool asServer)
  {
    valueText.text = next ;
    StartCoroutine( FlashValue() ) ;
  }
#endregion


#region Animation
  private IEnumerator FlashValue()
  {
    yield return new WaitUntil( () => !_valueFlashLock ) ;
    _valueFlashLock = true ;

    valueText.color = FLASH_COLOR ;

    yield return new WaitForSeconds( (float)TimeManager.TicksToTime(FLASH_TICKS) ) ;

    valueText.color = BASE_COLOR ;

    _valueFlashLock = false ;
  }
#endregion


#region MonoBehavior
private void OnDisable()
  {
    Label.OnChange -= OnLabelChange ;
    Value.OnChange -= OnValueChange ;
  }

  private IEnumerator DelayedInitialization()
  {
    Label.OnChange += OnLabelChange ;
    Value.OnChange += OnValueChange ;

    yield return null ;
    if( IsServerInitialized )
    {
      Value.Value = defaultValue ;
    }
  }

  private void Start()
  {
    StartCoroutine( DelayedInitialization() ) ;
  }
#endregion
}

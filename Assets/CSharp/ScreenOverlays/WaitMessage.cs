using TMPro;
using UnityEngine;


/**
  wait message doesn't appear for client when using networkhudcanvas
  because it's not a network object
  
  I will fix this later or never
*/
[RequireComponent(typeof(TextMeshProUGUI))]
public class WaitMessage : MonoBehaviour
{
  public void HideMe(GameEventContext ctx)
  {
    GetComponent<TextMeshProUGUI>().text = string.Empty ;
  }

  public void ShowMe(GameEventContext ctx)
  {
    GetComponent<TextMeshProUGUI>().text = "Waiting for co-op partner" ;
  }
}

using System;
using UnityEngine;
using TMPro;

[Serializable]
public class MenuConfigClientOrHost
{
  [Header("User Actions")]
  [SerializeField] private GameObject top ;

  public GameObject Top => top ;

  
  [Header("Scenario Actions")]
  [SerializeField] private TMP_InputField hostIP ;
  [SerializeField] private TMP_InputField hostPort ;
  
  public TMP_InputField IP => hostIP ;
  public TMP_InputField Port => hostPort ;
}
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using System.Collections;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SocialPlatforms.Impl;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System;

[Serializable]
public class WeaponInfo
{
  [SerializeField] WeaponScriptableObject weapon ;
  //[SerializeField] Transform origin ;


  public WeaponScriptableObject Weapon => weapon ;
  //public Transform Origin => origin ;
}
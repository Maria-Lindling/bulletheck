using UnityEngine;
using System;

[Serializable]
public class WeaponInfo
{
  [SerializeField] WeaponScriptableObject weapon ;
  //[SerializeField] Transform origin ;


  public WeaponScriptableObject Weapon => weapon ;
  //public Transform Origin => origin ;
}
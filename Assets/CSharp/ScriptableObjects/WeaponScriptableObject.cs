using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
  
  [SerializeField] ScriptableObject bulletPattern ;
  [SerializeField] double refireCooldown ;

  public IBulletPattern BulletPattern => bulletPattern as IBulletPattern ;

  public double RawCooldown => (float)refireCooldown ;
}

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
  
  [SerializeField] ScriptableObject bulletPattern ;
  [SerializeField] int refireCooldown ;

  private Cooldown _refireCooldown ;

  public IBulletPattern BulletPattern => bulletPattern as IBulletPattern ;
  public Cooldown RefireCooldown
  {
    get {
      if(_refireCooldown == null)
        Init() ;  
      return _refireCooldown ;
    }
    private set { _refireCooldown = value ; }
  }

  void Init()
  {
    RefireCooldown = new( new TimeSpan(0,0,0,0,refireCooldown) ) ;
    RefireCooldown.Restart() ;
    RefireCooldown.Shortcut() ;
  }
}

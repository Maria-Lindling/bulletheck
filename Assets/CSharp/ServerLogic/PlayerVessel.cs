using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using GameKit.Dependencies.Utilities;

public class PlayerVessel : NetworkBehaviour, IEntityController
{
#region Colors
  private readonly List<(Color Color,float Percentage)> _healthThresholds = new () {
    ( Color.cyan,   1.00f ),
    ( Color.green,  0.85f ),
    ( Color.yellow, 0.65f ),
    ( Color.red,    0.15f ),
    ( Color.black,  0.00f ),
  } ;
#endregion

#region SyncVar
  private readonly SyncVar<float> syncHtPoints = new() ;
  private readonly SyncVar<bool> isReady = new() ;

  public readonly SyncVar<Vector3> syncMove = new() ;
  public readonly SyncVar<Vector3> syncLook = new() ;

  public readonly SyncVar<bool> syncAtk1 = new() ;
  public readonly SyncVar<bool> syncAtk2 = new() ;
  public readonly SyncVar<bool> syncAtk3 = new() ;
#endregion


#region Fields
  private Vector2 _currentMovement ;
  private uint _primaryWeaponCooldownEnd = 0 ;
  private uint _secondaryWeaponCooldownEnd = 0 ;
  private uint _turretWeaponCooldownEnd = 0 ;
  private bool _isDespawning = false ;
#endregion


#region Properties
  public bool IsReady => isReady.Value ;
  public PlayerSelect Identity { get ; private set ; }
  public ForceSelection Force => force ;
#endregion


#region Unity Editor
  [SerializeField] private ForceSelection force = ForceSelection.Player ;
  [SerializeField] private VesselStats stats ;
  [SerializeField] private SpriteRenderer hurtSprite ;
  [SerializeField] private GameObject healthBar ;
  [SerializeField] private float minX = -8.75f ;
  [SerializeField] private float maxX = 8.75f ;
  [SerializeField] private float minY = -4.5f ;
  [SerializeField] private float maxY = 4.75f ;

  [SerializeField] private WeaponInfo primaryWeapon ;
  [SerializeField] private List<Transform> primaryWeaponOrigins ;

  [SerializeField] private WeaponInfo secondaryWeapon ;
  [SerializeField] private Transform secondaryWeaponOrigin ;

  [SerializeField] private WeaponInfo turretWeapon ;
  [SerializeField] private Transform turretWeaponOrigin ;
  [SerializeField] private GameObject turretObject ;

  private uint iFramesEnd = 0 ;
  private uint damageFlashEnd = 0 ;
#endregion


#region Server
  [Server]
  private void UpdatePosition()
  {
    transform.position = new Vector3(
      Mathf.Clamp( transform.position.x + _currentMovement.x, minX, maxX ),
      Mathf.Clamp( transform.position.y + _currentMovement.y, minY, maxY ),
      0
    ) ;
  }

  [Server]
  private void UpdateAutoAttack()
  {
    if( syncAtk1.Value && TimeManager.LocalTick >= _primaryWeaponCooldownEnd )
    {
      foreach( Transform attackOrigin in primaryWeaponOrigins )
      {
        primaryWeapon.Weapon.BulletPattern.Spawn( attackOrigin, gameObject ) ;
      }
      _primaryWeaponCooldownEnd = TimeManager.LocalTick + TimeManager.TimeToTicks( primaryWeapon.Weapon.RawCooldown ) ;
    }

    if( syncAtk2.Value && TimeManager.LocalTick >= _secondaryWeaponCooldownEnd )
    {
      secondaryWeapon.Weapon.BulletPattern.Spawn( secondaryWeaponOrigin, gameObject ) ;
      _secondaryWeaponCooldownEnd = TimeManager.LocalTick + TimeManager.TimeToTicks( secondaryWeapon.Weapon.RawCooldown ) ;
    }

    if( syncAtk3.Value && TimeManager.LocalTick >= _turretWeaponCooldownEnd )
    {
      turretWeapon.Weapon.BulletPattern.Spawn( turretWeaponOrigin, gameObject ) ;
      _turretWeaponCooldownEnd = TimeManager.LocalTick + TimeManager.TimeToTicks( turretWeapon.Weapon.RawCooldown ) ;
    }
  }
#endregion


#region SyncEvents
  private void OnHitPointsChanged(float prev, float next, bool isServer )
  {
    if( next >= prev )
      return ;

    float hitPercent = next / stats.HitPoints ; 
    for( int i = 0 ; i < _healthThresholds.Count - 1 ; i++ )
    {
      if( hitPercent <= _healthThresholds[i].Percentage )
      {
        float healthTierSize = _healthThresholds[i].Percentage - _healthThresholds[i+1].Percentage ;
        float healthWithinTier = (hitPercent - _healthThresholds[i+1].Percentage) / healthTierSize ;
        float missingHealthWithinTier = 1.0f - healthWithinTier ;

        healthBar.GetComponent<SpriteRenderer>().material.color = (
          _healthThresholds[i].Color   * healthWithinTier +
          _healthThresholds[i+1].Color * missingHealthWithinTier
        ) ;

        healthBar.transform.SetScale( new Vector3( Mathf.Clamp( hitPercent, 0.0f, 1.0f ), 1.0f, 1.0f ) ) ;
      }
    }
    
    if( TimeManager.LocalTick > damageFlashEnd )
      StartCoroutine( DamageFlash( stats.IFrames ) ) ;

    if( !isServer )
      return ;

    if( TimeManager.LocalTick > iFramesEnd )
      iFramesEnd = TimeManager.LocalTick + TimeManager.TimeToTicks( stats.IFrames ) ;

    if( next <= 0.0f )
    {
      GameEventSystem.PlayerDefeated.Invoke( new (gameObject) ) ;
      StartCoroutine( ShrinkAndDespawn() ) ;
    }
  }

  private void OnMoveChanged(Vector3 prev, Vector3 next, bool isServer)
  {
    if( !isServer )
      return ;

    _currentMovement = stats.MoveSpeed * (float)TimeManager.TickDelta * next ;
  }

  private void OnLookChanged(Vector3 prev, Vector3 next, bool isServer)
  {
    if( !isServer )
      return ;

    Vector3 turretpos = Camera.main.WorldToScreenPoint( turretObject.transform.position ) ;

    turretObject.transform.rotation = Quaternion.Euler( new Vector3(0,0,
      -Mathf.Atan2(
        next.x - turretpos.x,
        next.y - turretpos.y
        ) * Mathf.Rad2Deg )
    ) ;
  }

  private void OnIsReadyChanged(bool prev, bool next, bool isServer)
  {
    if( !isServer )
      return ;
    
  }

  private void OnAtk1Changed(bool prev, bool next, bool isServer)
  {
    if( !isServer )
      return ;
    
  }

  private void OnAtk2Changed(bool prev, bool next, bool isServer)
  {
    if( !isServer )
      return ;
    
  }

  private void OnAtk3Changed(bool prev, bool next, bool isServer)
  {
    if( !isServer )
      return ;
    
  }
#endregion


#region 
  private IEnumerator DamageFlash(double duration)
  {
    damageFlashEnd = TimeManager.LocalTick + TimeManager.TimeToTicks(duration) ;
    Color initialColor = hurtSprite.color ;
    while( TimeManager.LocalTick < damageFlashEnd )
    {
      hurtSprite.color = Color.white ;
      yield return new WaitForSeconds( (float)TimeManager.TicksToTime(3) ) ;

      hurtSprite.color = Color.black ;
      yield return new WaitForSeconds( (float)TimeManager.TicksToTime(3) ) ;
    }
    yield return null ;
    hurtSprite.color = initialColor ;
  }

  private IEnumerator ShrinkAndDespawn()
  {
    _isDespawning = true ;

    WaitForEndOfFrame wait = new() ;

    for( int i = 0; i < 4 ; i++ )
    {
      yield return wait ;
      transform.localScale = Vector3.one * (1.00f + i * 0.05f) ;
    }

    for( int i = 0; i < 25 ; i++ )
    {
      yield return wait ;
      transform.localScale = Vector3.one * (1.20f - i * 0.05f) ;
    }

    GameEventContext defeatCtx = new GameEventContextBuilder( default )
      .AddValue<bool>( syncHtPoints.Value <= 0.0f )
      .Build();
    
    GameEventSystem.VesselDespawned.Invoke( defeatCtx ) ;

    Despawn( DespawnType.Destroy ) ;
    Destroy( gameObject ) ;
  }
#endregion


#region Init
  private void OnDisable()
  {
    syncHtPoints.OnChange -= OnHitPointsChanged ;
    syncMove.OnChange     -= OnMoveChanged ;
    isReady.OnChange      -= OnIsReadyChanged ;
    syncLook.OnChange     -= OnLookChanged ;
    syncAtk1.OnChange     -= OnAtk1Changed ;
    syncAtk2.OnChange     -= OnAtk2Changed ;
    syncAtk3.OnChange     -= OnAtk3Changed ;

    if( IsServerInitialized )
    {
      if( TimeManager != null )
      {
        TimeManager.OnTick -= OnTick ;
      }
    }
  }

  private IEnumerator DelayedInit()
  {
    syncHtPoints.OnChange += OnHitPointsChanged ;
    syncMove.OnChange     += OnMoveChanged ;
    isReady.OnChange      += OnIsReadyChanged ;
    syncLook.OnChange     += OnLookChanged ;
    syncAtk1.OnChange     += OnAtk1Changed ;
    syncAtk2.OnChange     += OnAtk2Changed ;
    syncAtk3.OnChange     += OnAtk3Changed ;
    
    yield return null ;
    if( IsServerInitialized )
    {
      if( TimeManager != null )
      {
        TimeManager.OnTick += OnTick ;
      }

      syncHtPoints.Value = stats.HitPoints ;
    }
  }
#endregion


#region OnTick
  private void OnTick()
  {
    if( !IsServerInitialized )
      return ;

    UpdatePosition() ;
    UpdateAutoAttack() ;
  }
#endregion


#region MonoBehavior
  private void Start()
  {
    StartCoroutine( DelayedInit() ) ;
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage)
  {
    if( damage <= 0.0f || TimeManager.LocalTick < iFramesEnd || _isDespawning )
      return false ;

    syncHtPoints.Value -= damage ;

    GameEventSystem.PlayerTakeDamage.Invoke( new( gameObject ) ) ;

    return true ;
  }
#endregion
}

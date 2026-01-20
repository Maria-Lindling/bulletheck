using UnityEngine;
using FishNet.Object;
using System.Collections;

public class EnemyController : NetworkBehaviour, IEntityController
{


#region Unity Editor
  [SerializeField] private ForceSelection force = ForceSelection.Enemy ;
  [SerializeField] private int scoreValue = 3500 ;
  [SerializeField] private float hitPoints = 100.0f ;
  [SerializeField] private float damageIFrameTime = 0.0f ;
  [SerializeField] private SpriteRenderer hurtSprite ;
  [SerializeField] private float velocity = 0.750f ;

  [SerializeField] private WeaponInfo primaryWeapon ;
  [SerializeField] private Transform primaryWeaponOrigin ;
#endregion


#region
  private uint weaponCooldownEnd = 0 ;
  private uint iFramesEnd = 0 ;
  private uint damageFlashEnd = 0 ;

  private bool _isDespawning = false ;
#endregion


#region
  public ForceSelection Force => force ;
#endregion


#region Update
  private void UpdatePosition()
  {
    transform.position = new Vector3( transform.position.x, transform.position.y - velocity * (float)TimeManager.TickDelta, 0 ) ;
  }

  private void UpdateAutoAttack()
  {
    
    if( TimeManager.LocalTick >= weaponCooldownEnd )
    {
      primaryWeapon.Weapon.BulletPattern.Spawn( primaryWeaponOrigin, gameObject ) ;
      weaponCooldownEnd = TimeManager.LocalTick + TimeManager.TimeToTicks( primaryWeapon.Weapon.RawCooldown ) ;
    }
  }
#endregion


#region 
  private IEnumerator DamageFlash(double duration)
  {
    damageFlashEnd = TimeManager.LocalTick + TimeManager.TimeToTicks(duration) ;
    Color initialColor = hurtSprite.color ;
    while( TimeManager.LocalTick < damageFlashEnd )
    {
      ReColorHurtSprite( Color.white ) ;
      yield return new WaitForSeconds( (float)TimeManager.TicksToTime(3) ) ;

      ReColorHurtSprite( Color.black ) ;
      yield return new WaitForSeconds( (float)TimeManager.TicksToTime(3) ) ;
    }
    yield return null ;
    ReColorHurtSprite( initialColor ) ;
  }
  [ObserversRpc]
  private void ReColorHurtSprite(Color color)
  {
    hurtSprite.color = color ;
  }

  private IEnumerator ShrinkAndDespawn()
  {
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

    Despawn( DespawnType.Destroy ) ;
    Destroy( gameObject ) ;
  }
#endregion


#region Init
  private void OnDisable()
  {
    if( TimeManager != null )
    {
      TimeManager.OnTick -= OnTick ;
    }
  }

  private IEnumerator DelayedIsOwner()
  {
    yield return null ;

    if( TimeManager != null )
    {
      TimeManager.OnTick += OnTick ;
    }
  }
#endregion


#region OnTick
  private void OnTick()
  {
    UpdatePosition() ;
    UpdateAutoAttack() ;
  }
#endregion


#region MonoBehavior
  private void Start()
  {
    if( !IsServerInitialized )
    {
      Destroy( this ) ;
      return ;
    }

    StartCoroutine( DelayedIsOwner() ) ;
  }
#endregion


#region IEntityController
  public bool TryDamageEntity(float damage) 
  {
    if( damage <= 0.0f || TimeManager.LocalTick < iFramesEnd || _isDespawning )
      return false ;

    hitPoints -= damage ;

    if( TimeManager.LocalTick > damageFlashEnd )
      StartCoroutine( DamageFlash(1.5d) ) ;

    if( hitPoints <= 0.0f )
    {
      _isDespawning = true ;
      GameEventSystem.EnemyDefeated.Invoke(
        new GameEventContextBuilder( default )
          .AddValue<int>( scoreValue )
          .Build()
      ) ;
      StartCoroutine( ShrinkAndDespawn() ) ;
    }

    iFramesEnd = TimeManager.LocalTick + TimeManager.TimeToTicks( damageIFrameTime ) ;

    return true ;
  }
#endregion
}

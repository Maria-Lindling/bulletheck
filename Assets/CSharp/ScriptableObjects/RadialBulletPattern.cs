using UnityEngine;

[CreateAssetMenu(fileName = "RadialBulletPattern", menuName = "Scriptable Objects/RadialBulletPattern")]
public class RadialBulletPattern : ScriptableObject, IBulletPattern
{
  private const float DELAY_MULT = 2.3334f ;

  [SerializeField] GameObject bulletPrefab ;
  [SerializeField] float originOffset ;
  [SerializeField] float headingOffset ;
  [SerializeField] float speed ;
  [SerializeField] int numberOfLines ;
  [SerializeField] float gapBetweenLines ;
  [SerializeField] int bulletsPerLine ;
  [SerializeField] float gapBetweenBullets ;
  [SerializeField] float despawnTime ;

  public void Spawn(GameObject spawnPoint) => Spawn(spawnPoint,default) ;

  public void Spawn(GameObject spawnPoint, GameObject source) => Spawn(spawnPoint.transform,source) ;
  
  public void Spawn(Transform spawnPoint, GameObject source)
  {
    for( int i = 0; i<numberOfLines; i++ )
    {
      var (Origin, Heading) = GetSpawnInfo( spawnPoint, i ) ;
      
      for( int j = 0; j<bulletsPerLine; j++ )
      {
        GameEventContext ctx = new GameEventContextBuilder( source )
          .AddValue<Vector3>(Origin)
          .AddValue<Vector3>(Heading)
          .AddValue<float>( speed )
          .AddValue<float>( gapBetweenBullets / speed * j * DELAY_MULT )
          .AddValue<float>( despawnTime )
          .AddValue<GameObject>( bulletPrefab )
          .Build() ;

        GameEventSystem.SpawnBullet.Invoke( ctx ) ;
      }
    }
  }

  private (Vector3 Origin,Vector3 Heading) GetSpawnInfo(Transform transform, int line)
  {
    float delta = (-headingOffset + line * gapBetweenLines) * Mathf.Deg2Rad ;

    Vector3 heading = new Vector3(
      transform.right.x * Mathf.Cos(delta) - transform.right.y * Mathf.Sin(delta),
      transform.right.x * Mathf.Sin(delta) + transform.right.y * Mathf.Cos(delta),
      0.0f
    ) ;

    Vector3 origin  = transform.position + heading.normalized * originOffset ;
    
    return ( origin, heading ) ;
  }
}

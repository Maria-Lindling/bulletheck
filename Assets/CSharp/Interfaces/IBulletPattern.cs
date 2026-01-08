using UnityEngine;

public interface IBulletPattern
{
  public void Spawn(GameObject spawnPoint) ;
  public void Spawn(GameObject spawnPoint, GameObject source) ;
  public void Spawn(Transform spawnPoint, GameObject source) ;
}

using System;
using UnityEngine;

[Serializable]
public class EncounterEntry
{
  [SerializeField] float spawnDelay ;
  [SerializeField] int spawnPoint ;
  [SerializeField] GameObject enemyPrefab ;

  public float Delay => spawnDelay ;
  public int SpawnPoint => spawnPoint ;
  public GameObject Prefab => enemyPrefab ;
}
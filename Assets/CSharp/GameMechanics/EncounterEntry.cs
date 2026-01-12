
using System;
using FishNet.Demo.HashGrid;
using UnityEngine;

[Serializable]
internal class EncounterEntry
{
  [SerializeField] float spawnDelay ;
  [SerializeField] int spawnPoint ;
  [SerializeField] GameObject enemyPrefab ;

  internal float Delay => spawnDelay ;
  internal int SpawnPoint => spawnPoint ;
  internal GameObject Prefab => enemyPrefab ;
}
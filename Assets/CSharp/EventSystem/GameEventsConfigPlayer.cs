using Unity;
using System;
using UnityEngine.Events;
using UnityEngine;

[Serializable]
internal class GameEventsConfigPlayer
{
  [Header("User Actions")]
  [SerializeField] internal UnityEvent<GameEventContext> playerRegister ;
  [SerializeField] internal UnityEvent<GameEventContext> spawnVessel ;
  [SerializeField] internal UnityEvent<GameEventContext> vesselSpawned ;
  [SerializeField] internal UnityEvent<GameEventContext> vesselDespawned ;
  
  [Header("Scenario Actions")]
  [SerializeField] internal UnityEvent<GameEventContext> spawnBullet ;
  [SerializeField] internal UnityEvent<GameEventContext> scorePoint ;
}
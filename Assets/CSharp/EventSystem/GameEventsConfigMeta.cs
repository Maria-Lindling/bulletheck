using Unity;
using System;
using UnityEngine.Events;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[Serializable]
internal class GameEventsConfigMeta
{
  [Header("Client Status")]
  [SerializeField] internal UnityEvent<GameEventContext> clientConnect ;
  [SerializeField] internal UnityEvent<GameEventContext> clientDisconnect ;
  
  [Header("Scenario Status")]
  [SerializeField] internal UnityEvent<GameEventContext> scenarioBegin ;
  [SerializeField] internal UnityEvent<GameEventContext> scenarioEnd ;
  [SerializeField] internal UnityEvent<GameEventContext> pauseMenu ;
  
  [SerializeField] internal UnityEvent<GameEventContext> spawnEnemy ;
}
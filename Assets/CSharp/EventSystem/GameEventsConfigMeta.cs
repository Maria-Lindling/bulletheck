using Unity;
using System;
using UnityEngine.Events;
using UnityEngine;

[Serializable]
public class GameEventsConfigMeta
{
  [Header("Client Status")]
  [SerializeField] private UnityEvent<GameEventContext> clientConnect ;
  [SerializeField] private UnityEvent<GameEventContext> clientDisconnect ;
  
  [Header("Scenario Status")]
  [SerializeField] private UnityEvent<GameEventContext> scenarioBegin ;
  [SerializeField] private UnityEvent<GameEventContext> scenarioEnd ;
}
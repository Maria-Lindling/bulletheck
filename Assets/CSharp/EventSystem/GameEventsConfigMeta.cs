using System;
using UnityEngine.Events;
using UnityEngine;

[Serializable]
internal class GameEventsConfigMeta
{
  [Header("Client Status")]
  [SerializeField] internal UnityEvent<GameEventContext> clientConnect ;
  [SerializeField] internal UnityEvent<GameEventContext> clientDisconnect ;

  [Header("Info Status")]
  [SerializeField] internal UnityEvent<GameEventContext> hideMessage ;
  [SerializeField] internal UnityEvent<GameEventContext> clearMessage ;
  [SerializeField] internal UnityEvent<GameEventContext> setMessage ;
  [SerializeField] internal UnityEvent<GameEventContext> showMessage ;
  [SerializeField] internal UnityEvent<GameEventContext> submitScoreToDb ;
  [SerializeField] internal UnityEvent<GameEventContext> scoreSubmitted ;
  [SerializeField] internal UnityEvent<GameEventContext> refreshLeaderboard ;
  [SerializeField] internal UnityEvent<GameEventContext> switchInputMode ;
  
  [Header("Scenario Status")]
  [SerializeField] internal UnityEvent<GameEventContext> scenarioBegin ;
  [SerializeField] internal UnityEvent<GameEventContext> scenarioEnd ;
  [SerializeField] internal UnityEvent<GameEventContext> encounterEnd ;
  [SerializeField] internal UnityEvent<GameEventContext> pauseMenu ;
  
  [SerializeField] internal UnityEvent<GameEventContext> spawnEnemy ;
  [SerializeField] internal UnityEvent<GameEventContext> enemyDefeated ;
}
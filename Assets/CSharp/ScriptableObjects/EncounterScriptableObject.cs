using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterScriptableObject", menuName = "Scriptable Objects/Encounter")]
public class EncounterScriptableObject : ScriptableObject
{
  [SerializeField] private List<EncounterEntry> encounterData ;

  internal List<EncounterEntry> Data => encounterData ;
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterScriptableObject", menuName = "Scriptable Objects/Encounter")]
public class EncounterScriptableObject : ScriptableObject
{
  [SerializeField] private List<EncounterEntry> encounterData ;
  [SerializeField] private float extraDelay = 0.0f ;

  public List<EncounterEntry> Data => encounterData ;

  public float Duration {
    get
    {
      return encounterData.Sum( e => e.Delay ) + extraDelay ;
    }
  }
}

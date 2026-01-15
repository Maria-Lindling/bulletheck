
using System;
using UnityEngine;

[Serializable]
public class VesselStats
{
  [SerializeField] private float moveSpeed = 5.0f ;
  [SerializeField] private float hitPoints = 100.0f ;
  [SerializeField] private float damageIFrameTime = 1.5f ;

  public float HitPoints => hitPoints ;
  public float MoveSpeed => moveSpeed ;
  public float IFrames => damageIFrameTime ;
}
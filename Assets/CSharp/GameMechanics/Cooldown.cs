using System;
using UnityEngine;

public class Cooldown : Timepiece
{
  #region Raw Data
  private readonly TimeSpan _cooldownDuration;
  #endregion

  #region Flags
  public new bool IsRunning => base.IsRunning;
  public bool InProgress => (DeltaTime < _cooldownDuration) && IsRunning;
  public bool IsComplete => DeltaTime >= _cooldownDuration;
  #endregion

  #region Start/Stop
  public void Reset() => ResetTimer();

  public void Start() => StartTimer();

  public void Restart() => RestartTimer();

  public void Stop() => StopTimer();

  public void Shortcut() => AddTime(_cooldownDuration);
  #endregion

  //public void DebugStatus()
  //{
  //  Debug.Log($"IsRunning: {IsRunning}, DeltaTime: {DeltaTime.Milliseconds}ms, Duration: {_cooldownDuration.Milliseconds}ms");
  //}

  #region Constructors
  private Cooldown(bool active) : base(active)
  {

  }


  public Cooldown(TimeSpan duration) : this(duration, false)
  {
  
  }

  public Cooldown(TimeSpan duration, bool active) : this(active)
  {
    _cooldownDuration = duration;
  }
  #endregion
}
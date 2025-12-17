using System;

public class Timepiece
{
  #region Raw Data
  private DateTime _timekeepingStart;
  private DateTime _timekeepingStop;
  private TimeSpan _addedTime;
  #endregion

  #region Derived Data
  protected TimeSpan DeltaTime
  {
    get
    {
      if (_timerIsRunning)
      {
        return DateTime.Now - CombinedTime ;
      }
      else if (_timekeepingStop != default)
      {
        return _timekeepingStop - CombinedTime;
      }
      else
      {
        return TimeSpan.Zero;
      }  
    }
  }
  #endregion

  #region Flags
  private bool _timerIsRunning;
  #endregion

  #region Properties
  private DateTime CombinedTime => _timekeepingStart.Add(_addedTime) ;
  protected bool IsRunning => _timerIsRunning;
  protected bool IsStopped => !_timerIsRunning;
  protected bool IsPaused => IsStopped && _timekeepingStop != default; 
  #endregion

  #region Start/Stop
  protected void ResetTimer()
  {
    _timerIsRunning = false;
    _timekeepingStart = default;
    _timekeepingStop = default;
    _addedTime = TimeSpan.Zero ;
  }

  protected void StartTimer()
  {
    _timerIsRunning = true;
    _timekeepingStop = default;
  }

  protected void RestartTimer()
  {
    ResetTimer();
    StartTimer();
    _timekeepingStart = DateTime.Now;
  }

  protected void StopTimer()
  {
    _timerIsRunning = false;
    _timekeepingStop = DateTime.Now;
  }

  protected void AddTime(TimeSpan value)
  {
    _addedTime += value ;
  }
  #endregion

  #region Constructors
  protected Timepiece(bool active)
  {
    if (active)
    {
      RestartTimer();
    }
    else
    {
      ResetTimer();
    }
  }

  public Timepiece() : this(false)
  {

  }
  #endregion
}
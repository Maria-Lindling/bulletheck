using UnityEngine;
using FishNet.Object;
using UnityEngine.Events;
using System;
using System.Collections;
using Mono.Cecil.Cil;

public class GameEventContext
{
#region Fields
  GameEventContextStatus _isSealed ;
#endregion

#region Value Fields
  private string _evtValueString ;
  private float _evtValueFloat ;
  private int _evtValueInt ;
  private bool _evtValueBool ;
  private Vector2 _evtValueVector2 ;
  private Vector3 _evtValueVector3 ;
#endregion


#region Properties
  public GameObject Source { get ; private set ; }
  private bool IsSealed => _isSealed == GameEventContextStatus.Sealed ;
#endregion


#region Seal
  public GameEventContext Seal()
  {
    _isSealed = GameEventContextStatus.Sealed ;
    return this ;
  }
#endregion


#region AddValue
  public GameEventContext AddValue<T>(T value)
  {
    if( IsSealed )
      return this ;
    
    switch( value )
    {
      case string v :
        _evtValueString = v ;
        // Debug.Log($"AddValue<string>({v})") ;
        break ;

      case float v :
        _evtValueFloat = v ;
        // Debug.Log($"AddValue<float>({v})") ;
        break ;

      case int v :
        _evtValueInt = v ;
        // Debug.Log($"AddValue<int>({v})") ;
        break ;
        
      case bool v :
        _evtValueBool = v ;
        // Debug.Log($"AddValue<bool>({v})") ;
        break ;
        
      case Vector2 v :
        _evtValueVector2 = v ;
        // Debug.Log($"AddValue<Vector2>({v})") ;
        break ;
        
      case Vector3 v :
        _evtValueVector3 = v ;
        // Debug.Log($"AddValue<Vector3>({v})") ;
        break ;
      
      default :
        throw new ArgumentException($"The type '{typeof(T)}' is not an accepted type parameter. Accepted type parameters are {typeof(string)}, {typeof(float)}, {typeof(int)}, {typeof(bool)}, {typeof(Vector2)} and {typeof(Vector3)}") ;
    }
    return this ;
  }
#endregion


#region ReadValue
  public T ReadValue<T>()
  {
    T value ;
    switch ( typeof(T) )
    {
      case Type t when t == typeof(string) :
        value = (T)Convert.ChangeType(_evtValueString, typeof(T)) ;
        // Debug.Log($"ReadValue<string> == {value}") ;
        break ;

      case Type t when t == typeof(float) :
        value = (T)Convert.ChangeType(_evtValueFloat, typeof(T)) ;
        // Debug.Log($"ReadValue<float> == {value}") ;
        break ;

      case Type t when t == typeof(int) :
        value = (T)Convert.ChangeType(_evtValueInt, typeof(T)) ;
        // Debug.Log($"ReadValue<int> == {value}") ;
        break ;

      case Type t when t == typeof(bool) :
        value = (T)Convert.ChangeType(_evtValueBool, typeof(T)) ;
        // Debug.Log($"ReadValue<bool> == {value}") ;
        break ;

      case Type t when t == typeof(Vector2) :
        value = (T)Convert.ChangeType(_evtValueVector2, typeof(T)) ;
        // Debug.Log($"ReadValue<Vector2> == {value}") ;
        break ;

      case Type t when t == typeof(Vector3) :
        value = (T)Convert.ChangeType(_evtValueVector3, typeof(T)) ;
        // Debug.Log($"ReadValue<Vector3> == {value}") ;
        break ;
      
      default:
        throw new ArgumentException($"The type '{typeof(T)}' is not an accepted type parameter. Accepted type parameters are {typeof(string)}, {typeof(float)}, {typeof(int)}, {typeof(bool)}, {typeof(Vector2)} and {typeof(Vector3)}") ;
    }
    return value ;
  }

  public bool TryReadValue<T>(out T value)
  {
    try
    {
      value = ReadValue<T>() ;
      return true ;
    }
    catch (ArgumentException e)
    {
      Debug.Log(e.Message) ;
    }
    value = default ;
    return false ;
  }
#endregion


#region (2) Constructor

  public GameEventContext(GameObject source, string value1, float value2) : this(source,value1)
  {
    _evtValueFloat = value2 ;
  }

  public GameEventContext(GameObject source, float value1, string value2) : this(source,value1)
  {
    _evtValueString = value2 ;
  }
  
  public GameEventContext(GameObject source, string value1, int value2) : this(source,value1)
  {
    _evtValueInt = value2 ;
  }

  public GameEventContext(GameObject source, int value1, string value2) : this(source,value1)
  {
    _evtValueString = value2 ;
  }
  
  public GameEventContext(GameObject source, string value1, bool value2) : this(source,value1)
  {
    _evtValueBool = value2 ;
  }

  public GameEventContext(GameObject source, bool value1, string value2) : this(source,value1)
  {
    _evtValueString = value2 ;
  }
#endregion


#region (1) Constructor
  public GameEventContext(GameObject source, string value) : this(source,GameEventContextStatus.Sealed)
  {
    _evtValueString = value ;
  }
  
  public GameEventContext(GameObject source, float value) : this(source,GameEventContextStatus.Sealed)
  {
    _evtValueFloat = value ;
  }
  
  public GameEventContext(GameObject source, int value) : this(source,GameEventContextStatus.Sealed)
  {
    _evtValueInt = value ;
  }
  
  public GameEventContext(GameObject source, bool value) : this(source,GameEventContextStatus.Sealed)
  {
    _evtValueBool = value ;
  }
  
  public GameEventContext(GameObject source, Vector2 value) : this(source,GameEventContextStatus.Sealed)
  {
    _evtValueVector2 = value ;
  }
  
  public GameEventContext(GameObject source, Vector3 value) : this(source,GameEventContextStatus.Sealed)
  {
    _evtValueVector3 = value ;
  }
#endregion


#region (0) Constructor
  /// <summary>
  /// <para>The lowest-level constructor for OwnGameEventContext.</para>
  /// </summary>
  /// <param name="source">The dispatcher of the event.</param>
  /// <param name="status">The state in which the context is initialized.</param>
  public GameEventContext(GameObject source, GameEventContextStatus status = GameEventContextStatus.Sealed)
  {
    _isSealed = status ;
    Source = source ;
  }
#endregion
}
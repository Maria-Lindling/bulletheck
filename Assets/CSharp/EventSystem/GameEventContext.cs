using UnityEngine;
using FishNet.Object;
using UnityEngine.Events;
using System;
using System.Collections;
using Mono.Cecil.Cil;

public class GameEventContext
{
#region Value Fields
  private readonly string[] _evtValuesString ;
  private readonly float[] _evtValuesFloat ;
  private readonly int[] _evtValuesInt ;
  private readonly bool[] _evtValuesBool ;
  private readonly Vector2[] _evtValuesVector2 ;
  private readonly Vector3[] _evtValuesVector3 ;
  private readonly Quaternion[] _evtValuesQuaternion ;
  private readonly GameObject[] _evtValuesPrefab ;
  private readonly PlayerSelect _evtValuePlayerSelect ;
#endregion


#region Properties
  public GameObject Source { get ; private set ; }
#endregion


#region ReadValue
  public T ReadValue<T>(int index = 0)
  {
    T value ;
    switch ( typeof(T) )
    {
      case Type t when t == typeof(string) :
        value = (T)Convert.ChangeType(_evtValuesString[index], typeof(T)) ;
        // Debug.Log($"ReadValue<string> == {value}") ;
        break ;

      case Type t when t == typeof(float) :
        value = (T)Convert.ChangeType(_evtValuesFloat[index], typeof(T)) ;
        // Debug.Log($"ReadValue<float> == {value}") ;
        break ;

      case Type t when t == typeof(int) :
        value = (T)Convert.ChangeType(_evtValuesInt[index], typeof(T)) ;
        // Debug.Log($"ReadValue<int> == {value}") ;
        break ;

      case Type t when t == typeof(bool) :
        value = (T)Convert.ChangeType(_evtValuesBool[index], typeof(T)) ;
        // Debug.Log($"ReadValue<bool> == {value}") ;
        break ;

      case Type t when t == typeof(Vector2) :
        value = (T)Convert.ChangeType(_evtValuesVector2[index], typeof(T)) ;
        // Debug.Log($"ReadValue<Vector2> == {value}") ;
        break ;

      case Type t when t == typeof(Vector3) :
        value = (T)Convert.ChangeType(_evtValuesVector3[index], typeof(T)) ;
        // Debug.Log($"ReadValue<Vector3> == {value}") ;
        break ;

      case Type t when t == typeof(Quaternion) :
        value = (T)Convert.ChangeType(_evtValuesQuaternion[index], typeof(T)) ;
        // Debug.Log($"ReadValue<Quaternion> == {value}") ;
        break ;

      case Type t when t == typeof(GameObject) :
        value = (T)Convert.ChangeType(_evtValuesPrefab[index], typeof(T)) ;
        break ;

      case Type t when t == typeof(PlayerSelect) :
        value = (T)Convert.ChangeType(_evtValuePlayerSelect, typeof(T)) ;
        break ;
      
      default:
        throw new ArgumentException($"The type '{typeof(T)}' is not an accepted type parameter. Accepted type parameters are {typeof(string)}, {typeof(float)}, {typeof(int)}, {typeof(bool)}, {typeof(Vector2)}, {typeof(Vector3)}, {typeof(PlayerSelect)} and {typeof(GameObject)}") ;
    }
    return value ;
  }

  public T[] ReadValues<T>()
  {
    T[] values ;
    switch ( typeof(T) )
    {
      case Type t when t == typeof(string) :
        values = (T[])Convert.ChangeType(_evtValuesString, typeof(T[])) ;
        // Debug.Log($"ReadValue<string> == {value}") ;
        break ;

      case Type t when t == typeof(float) :
        values = (T[])Convert.ChangeType(_evtValuesFloat, typeof(T[])) ;
        // Debug.Log($"ReadValue<float> == {value}") ;
        break ;

      case Type t when t == typeof(int) :
        values = (T[])Convert.ChangeType(_evtValuesInt, typeof(T[])) ;
        // Debug.Log($"ReadValue<int> == {value}") ;
        break ;

      case Type t when t == typeof(bool) :
        values = (T[])Convert.ChangeType(_evtValuesBool, typeof(T[])) ;
        // Debug.Log($"ReadValue<bool> == {value}") ;
        break ;

      case Type t when t == typeof(Vector2) :
        values = (T[])Convert.ChangeType(_evtValuesVector2, typeof(T[])) ;
        // Debug.Log($"ReadValue<Vector2> == {value}") ;
        break ;

      case Type t when t == typeof(Vector3) :
        values = (T[])Convert.ChangeType(_evtValuesVector3, typeof(T[])) ;
        // Debug.Log($"ReadValue<Vector3> == {value}") ;
        break ;

      case Type t when t == typeof(Quaternion) :
        values = (T[])Convert.ChangeType(_evtValuesQuaternion, typeof(T[])) ;
        // Debug.Log($"ReadValue<Quaternion> == {value}") ;
        break ;

      case Type t when t == typeof(GameObject) :
        values = (T[])Convert.ChangeType(_evtValuesQuaternion, typeof(T[])) ;
        // Debug.Log($"ReadValue<Quaternion> == {value}") ;
        break ;
      
      default:
        throw new ArgumentException($"The type '{typeof(T)}' is not an accepted type parameter. Accepted type parameters are {typeof(string)}, {typeof(float)}, {typeof(int)}, {typeof(bool)}, {typeof(Vector2)}, {typeof(Vector3)}, {typeof(PlayerSelect)} and {typeof(GameObject)}") ;
    }
    return values ;
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
      //Debug.Log(e.Message) ;
    }
    catch (IndexOutOfRangeException e)
    {
      //Debug.Log(e.Message) ;
    }
    value = default ;
    return false ;
  }

  public bool TryReadValue<T>(int index, out T value)
  {
    try
    {
      value = ReadValue<T>(index) ;
      return true ;
    }
    catch (ArgumentException e)
    {
      //Debug.Log(e.Message) ;
    }
    catch (IndexOutOfRangeException e)
    {
      //Debug.Log(e.Message) ;
    }
    value = default ;
    return false ;
  }
#endregion


#region (n) Constructor
  /// <summary>
  /// <para>The lowest-level constructor for OwnGameEventContext.</para>
  /// </summary>
  /// <param name="source">The dispatcher of the event.</param>
  /// <param name="status">The state in which the context is initialized.</param>
  public GameEventContext(
    GameObject source,
    string[] strings,
    float[] floats,
    int[] ints,
    bool[] bools,
    Vector2[] vector2s,
    Vector3[] vector3s,
    Quaternion[] quaternions,
    GameObject[] prefabs
  ) : this(source)
  {
    _evtValuesString  = strings ;
    _evtValuesFloat   = floats ;
    _evtValuesInt     = ints ;
    _evtValuesBool    = bools ;
    _evtValuesVector2 = vector2s ;
    _evtValuesVector3 = vector3s ;
    _evtValuesQuaternion = quaternions ;
    _evtValuesPrefab = prefabs ;

    //Debug.Log( $"{_evtValuesFloat[0]}, {_evtValuesVector3[0]}, {_evtValuesVector3[1]}" ) ;
  }
#endregion


#region (0) Constructor
  /// <summary>
  /// <para>The lowest-level constructor for OwnGameEventContext.</para>
  /// </summary>
  /// <param name="source">The dispatcher of the event.</param>
  /// <param name="status">The state in which the context is initialized.</param>
  public GameEventContext(GameObject source)
  {
    if( source == null )
    {
      _evtValuePlayerSelect = PlayerSelect.None ;
      Source = default ;
      return ;
    }
    
    Source = source ;

    if( Source.TryGetComponent(out PlayerVessel player) )
    {
      _evtValuePlayerSelect = player.Identity ;
    }
    else
    {
      _evtValuePlayerSelect = PlayerSelect.None ;
    }
  }
#endregion
}
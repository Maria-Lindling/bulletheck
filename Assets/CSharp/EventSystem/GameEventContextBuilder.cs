using System;
using System.Collections.Generic;
using UnityEngine ;

public class GameEventContextBuilder
{

#region Value Fields
  private readonly GameObject _source ;
  private readonly List<string> _evtValuesString = new() ;
  private readonly List<float> _evtValuesFloat = new() ;
  private readonly List<int> _evtValuesInt = new() ;
  private readonly List<bool> _evtValuesBool = new() ;
  private readonly List<Vector2> _evtValuesVector2 = new() ;
  private readonly List<Vector3> _evtValuesVector3 = new() ;
  private readonly List<Quaternion> _evtValuesQuaternion = new() ;
  private readonly List<GameObject> _evtValuesPrefab = new() ;
  private PlayerSelect _evtValuePlayerSelect ;
#endregion


#region AddValue
  public GameEventContextBuilder AddValue<T>(T value)
  {
    switch( value )
    {
      case string v :
        _evtValuesString.Add(v) ;
        // Debug.Log($"AddValue<string>({v})") ;
        break ;

      case float v :
        _evtValuesFloat.Add(v) ;
        // Debug.Log($"AddValue<float>({v})") ;
        break ;

      case int v :
        _evtValuesInt.Add(v) ;
        // Debug.Log($"AddValue<int>({v})") ;
        break ;
        
      case bool v :
        _evtValuesBool.Add(v) ;
        // Debug.Log($"AddValue<bool>({v})") ;
        break ;
        
      case Vector2 v :
        _evtValuesVector2.Add(v) ;
        // Debug.Log($"AddValue<Vector2>({v})") ;
        break ;
        
      case Vector3 v :
        _evtValuesVector3.Add(v) ;
        // Debug.Log($"AddValue<Vector3>({v})") ;
        break ;
        
      case Quaternion v :
        _evtValuesQuaternion.Add(v) ;
        // Debug.Log($"AddValue<Quaternion>({v})") ;
        break ;
      
      case GameObject v:
        _evtValuesPrefab.Add(v) ;
        break ;

      case PlayerSelect v:
        _evtValuePlayerSelect = v ;
        break ;
      
      default :
        throw new ArgumentException($"The type '{typeof(T)}' is not an accepted type parameter. Accepted type parameters are {typeof(string)}, {typeof(float)}, {typeof(int)}, {typeof(bool)}, {typeof(Vector2)}, {typeof(Vector3)}, {typeof(PlayerSelect)} and {typeof(GameObject)}") ;
    }
    return this ;
  }
#endregion

  public GameEventContext Build()
  {
    return new(
      _source,
      _evtValuesString.ToArray(),
      _evtValuesFloat.ToArray(),
      _evtValuesInt.ToArray(),
      _evtValuesBool.ToArray(),
      _evtValuesVector2.ToArray(),
      _evtValuesVector3.ToArray(),
      _evtValuesQuaternion.ToArray(),
      _evtValuesPrefab.ToArray()
    ) ;
  }

#region Constructor
  public GameEventContextBuilder(GameObject source)
  {
    _source = source ;
  }
#endregion
}

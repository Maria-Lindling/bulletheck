using UnityEngine;
using FishNet.Object;
using System.Collections.Generic;
using FishNet.Utility.Performance;
using FishNet;
using System;

[RequireComponent(typeof(DefaultObjectPool))]
public class PoolBoy : NetworkBehaviour
{
  [Serializable]
  private class PrewarmOrder
  {
    [SerializeField] public int count ;
    [SerializeField] public GameObject prefab ; 
  }
  
  [SerializeField] private List<PrewarmOrder> prewarmOrders ;

  private DefaultObjectPool _pool ;

  public override void OnStartNetwork()
  {
    base.OnStartNetwork() ;

    WarmUpPool() ;
  }

  private void WarmUpPool()
  {
    _pool = GetComponent<DefaultObjectPool>() ;

    foreach( PrewarmOrder prewarmOrder in prewarmOrders)
    {
      _pool.StorePrefabObjects( prewarmOrder.prefab.GetComponent<NetworkObject>(), prewarmOrder.count, IsServerInitialized ) ;
    }
  }
}

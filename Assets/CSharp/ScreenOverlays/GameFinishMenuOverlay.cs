using UnityEngine;
using FishNet.Object;
using GameKit.Dependencies.Utilities;
using UnityEngine.UI;
using TMPro;
using FishNet.Transporting.Tugboat;
using System;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using FishNet.Managing;
using System.Net;
using FishNet.Authenticating;
using System.Net.Sockets;

public class GameFinishMenuOverlay : NetworkBehaviour
{
#region Unity Editor
  [SerializeField] GameObject topMenu ;
  [SerializeField] TextMeshProUGUI victoryMessage ;
  [SerializeField] GameObject background ;

  [Header("Submit Score")]
  [SerializeField] GameObject submitScoreMenu ;
  [SerializeField] TMP_InputField submitScoreLocalPlayerName ;
  [SerializeField] Button submitScoreButton ;

  [Header("Leaderboard")]
  [SerializeField] GameObject leaderboardMenu ;
  [SerializeField] GameObject leaderboardListing ;
  [SerializeField] GameObject listingEntryPrefab ;
#endregion


#region Events
  public void OnClickButtonTopMenu()
  {
    leaderboardMenu.SetActive(false) ;
    submitScoreMenu.SetActive(false) ;
    topMenu.SetActive(true) ;
  }

  public void OnClickButtonLeaderboardMenu()
  {
    submitScoreMenu.SetActive(false) ;
    topMenu.SetActive(false) ;
    
    leaderboardMenu.SetActive(true) ;
  }

  public void OnClickButtonSubmitScoreMenu()
  {
    leaderboardMenu.SetActive(false) ;
    topMenu.SetActive(false) ;
    
    submitScoreMenu.SetActive(true) ;
  }

  public void OnContinuePlaying()
  {
    // clear game objects and restart encounter
  }

  public void OnExitGame()
  {
    // shut down
  }
#endregion


#region 
  public void OnSubmitScore()
  {
    
  }
#endregion

#region Game Events
  public void OnEncounterEnd(GameEventContext ctx)
  {
    OpenForAll( ctx.ReadValue<int>() ) ;
  }

  [ObserversRpc]
  private void OpenForAll(int score)
  {
    topMenu.SetActive(true) ;
    background.SetActive(true) ;

    victoryMessage.text = victoryMessage.text.Replace( "[[SCORE]]", score.ToString("###,###,###,##0") ) ;
  }
#endregion 

  private void Awake()
  {
    leaderboardMenu.SetActive(false) ;
    submitScoreMenu.SetActive(false) ;
    topMenu.SetActive(false) ;
    background.SetActive(false) ;
  }
}

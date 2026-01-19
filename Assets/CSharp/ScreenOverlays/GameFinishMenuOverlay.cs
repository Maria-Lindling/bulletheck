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
using FishNet.Object.Synchronizing;
using UnityEngine.SocialPlatforms.Impl;

public class GameFinishMenuOverlay : NetworkBehaviour
{
#region Unity Editor
  [SerializeField] GameObject topMenu ;
  [SerializeField] Button submitScoreTopButton ;
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


#region sync
  private readonly SyncVar<string> syncHostName = new( string.Empty ) ;
  private readonly SyncVar<string> syncClientName = new( string.Empty ) ;
  private readonly SyncVar<int> syncSessionScore = new( -1 ) ;
#endregion


#region 
  public Transform LeaderboardTransform => leaderboardListing.transform ;
  private bool _scoreSubmitted = false ;
  private bool ReadyToSubmitScore => syncHostName.Value     != string.Empty &&
                                     syncClientName.Value   != string.Empty &&
                                     syncSessionScore.Value >= 0 &&
                                     !_scoreSubmitted ;
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
    leaderboardListing.SetActive(true) ;
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
    SubmitName(WorldManager.LocalPlayer, submitScoreLocalPlayerName.text) ;
    submitScoreLocalPlayerName.interactable = false ;
    submitScoreButton.interactable = false ;
    submitScoreButton.GetComponentInChildren<TextMeshProUGUI>().text = "Submitting..." ;
  }
  [ServerRpc(RequireOwnership = false)]
  private void SubmitName(PlayerSelect playerSelect, string name)
  {
    switch( playerSelect )
    {
      case PlayerSelect.Player1:
        syncHostName.Value = name ;
        break;

      case PlayerSelect.Player2:
        syncClientName.Value = name ;
        break;
      
      default:
        return;
    }
  }
#endregion


#region Sync Events
  private void OnHostNameChange(string prev, string next, bool isServer)
  {
    TrySubmitScore(isServer) ;
  }
  
  private void OnClientNameChange(string prev, string next, bool isServer)
  {
    TrySubmitScore(isServer) ;
  }
  
  private void OnSessionScoreChange(int prev, int next, bool isServer)
  {
    TrySubmitScore(isServer) ;
  }

  private void TrySubmitScore(bool isServer)
  {
    if( !ReadyToSubmitScore )
      return ;
    if( isServer )
    {
      _scoreSubmitted = true ;
      GameEventSystem.SubmitScoreToDb.Invoke(
        new GameEventContextBuilder( gameObject )
        .AddValue<string>( syncHostName.Value   )
        .AddValue<string>( syncClientName.Value )
        .AddValue<int>( syncSessionScore.Value  )
        .Build()
      ) ;
    }
    else
    {
      submitScoreButton.GetComponentInChildren<TextMeshProUGUI>().text = "Submitted!" ;
      submitScoreTopButton.interactable = false ;

      if( submitScoreMenu.activeSelf )
        OnClickButtonLeaderboardMenu() ;
    }
  }
#endregion


#region Game Events
  public void OnEncounterEnd(GameEventContext ctx)
  {
    GameEventSystem.SwitchInputMode.Invoke( new(gameObject) ) ;

    GameEventSystem.RefreshLeaderboard.Invoke( new GameEventContextBuilder( gameObject )
      .AddValue<GameObject>( listingEntryPrefab )
      .AddValue<GameObject>( leaderboardListing )
      .Build() ) ;

    syncSessionScore.Value = ctx.ReadValue<int>() ;
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


  private void OnDisable()
  {
    syncHostName.OnChange     -= OnHostNameChange ;
    syncClientName.OnChange   -= OnClientNameChange ;
    syncSessionScore.OnChange -= OnSessionScoreChange ;
  }

  private void Awake()
  {
    leaderboardMenu.SetActive(false) ;
    submitScoreMenu.SetActive(false) ;
    topMenu.SetActive(false) ;
    background.SetActive(false) ;
  }

  private void Start()
  {
    syncHostName.OnChange     += OnHostNameChange ;
    syncClientName.OnChange   += OnClientNameChange ;
    syncSessionScore.OnChange += OnSessionScoreChange ;

    if( IsServerInitialized )
    {
      leaderboardMenu.SetActive(true) ;
      for(int i = 0 ; i < leaderboardListing.transform.childCount ; i++ )
      {
        Spawn( leaderboardListing.transform.GetChild(i).gameObject ) ;
      }
      leaderboardMenu.SetActive(false) ;
    }
  }
}

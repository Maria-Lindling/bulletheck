
using FishNet.Object;
using TMPro;
using UnityEngine;

public class LeaderboardEntry : NetworkBehaviour
{
  [SerializeField] private TextMeshProUGUI hostText ;
  [SerializeField] private TextMeshProUGUI clientText ;
  [SerializeField] private TextMeshProUGUI scoreText ;

  public void SetValues(string host, string client, int score)
  {
    hostText.text = host ;
    clientText.text = client ;
    scoreText.text = score.ToString("###,###,###,##0") ;
  }
}
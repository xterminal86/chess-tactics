using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverseer : MonoSingleton<GameOverseer> 
{
  public Sprite[] UnitsSprites;

  public GameObject[] CommandPointSprites;

  public GameObject GameOverGroup;
  public TMP_Text GameOverText;

  PlayerType _playerTurn = PlayerType.PLAYER1;
  public PlayerType PlayerTurn
  {
    get { return _playerTurn; }
    set
    {
      _playerTurn = value;
      TurnIndicator.text = _playerTurn.ToString();
    }
  }

  public TMP_Text TurnIndicator;

  public int CommandPoints = 3;

  public override void Initialize()
  {
    PlayerTurn = PlayerType.PLAYER1;
  }

  public void SpendCommandPoints(int pointsToSpend)
  {
    CommandPoints -= pointsToSpend;

    foreach (var star in CommandPointSprites)
    {
      star.SetActive(false);
    }

    for (int i = 0; i < CommandPoints; i++)
    {
      CommandPointSprites[i].SetActive(true);
    }

    if (CommandPoints == 0)
    {
      TurnDone();
    }
  }

  public void TurnDone()
  {
    CommandPoints = 3;

    for (int i = 0; i < CommandPoints; i++)
    {
      CommandPointSprites[i].SetActive(true);
    }
   
    PlayerTurn = (PlayerTurn == PlayerType.PLAYER1) ? PlayerType.PLAYER2 : PlayerType.PLAYER1;
  }

  public bool IsGameOver = false;

  public void SetGameOver(string gameOverText)
  {
    GameOverGroup.SetActive(true);
    GameOverText.text = gameOverText;
    IsGameOver = true;
  }

  public void RestartGameHandler()
  {
    IsGameOver = false;
    GameOverGroup.SetActive(false);
    PlayerTurn = PlayerType.PLAYER1;
    CommandPoints = 3;

    for (int i = 0; i < CommandPoints; i++)
    {
      CommandPointSprites[i].SetActive(true);
    }

    SceneManager.LoadScene("main");
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverseer : MonoSingleton<GameOverseer> 
{
  public Sprite[] UnitsSprites;

  public GameObject[] CommandPointSprites;

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

  public Unit SelectedUnit;

  public TMP_Text TurnIndicator;

  public int CommandPoints = 3;

  public override void Initialize()
  {
    PlayerTurn = PlayerType.PLAYER1;
  }

  public void SpendCommandPoints(int pointsToSpend)
  {
    CommandPoints -= pointsToSpend;

    int counter = 0;
    for (int i = CommandPointSprites.Length - 1; i >= 0; i--)
    {
      if (CommandPointSprites[i].activeSelf)
      {
        CommandPointSprites[i].SetActive(false);

        counter++;
        if (counter == pointsToSpend)
        {
          break;
        }
      }
    }
  }

  public void TurnDone()
  {
    CommandPoints = 3;

    for (int i = 0; i < CommandPointSprites.Length; i++)
    {
      CommandPointSprites[i].SetActive(true);
    }
   
    PlayerTurn = (PlayerTurn == PlayerType.PLAYER1) ? PlayerType.PLAYER2 : PlayerType.PLAYER1;
  }
}

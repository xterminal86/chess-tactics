﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverseer : MonoSingleton<GameOverseer> 
{
  public Sprite[] UnitsSprites;
  
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

  public override void Initialize()
  {
    PlayerTurn = PlayerType.PLAYER1;
  }
}

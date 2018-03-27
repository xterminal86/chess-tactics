using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverseer : MonoSingleton<GameOverseer> 
{
  public PlayerType PlayerTurn = PlayerType.PLAYER1;
}

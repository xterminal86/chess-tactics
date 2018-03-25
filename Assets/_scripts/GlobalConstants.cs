using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public delegate void Callback();
public delegate void CallbackO(object sender);
public delegate void CallbackB(bool arg);
public delegate void CallbackC(Collider arg);

public static class GlobalConstants 
{ 
  public static Dictionary<UnitType, int> UnitHealthByType = new Dictionary<UnitType, int>()
  {
    { UnitType.PAWN, 2 },
    { UnitType.BISHOP, 3 },
    { UnitType.KNIGHT, 3 },
    { UnitType.ROOK, 4 },
    { UnitType.QUEEN, 4 },
    { UnitType.KING, 3 },
  };
}




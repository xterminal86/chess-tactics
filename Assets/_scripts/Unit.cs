using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour 
{
  public Text UnitText;
  public Image UnitImage;
  public Image HealthBar;

  public UnitType ThisUnitType;
  public PlayerType Owner;

  public int Health = 1;
  public int Damage = 1;

  public bool MovedFirstTime = false;

  Vector2Int _position = Vector2Int.zero;
  public Vector2Int Position
  {
    get { return _position; }
  }

  Dictionary<UnitType, string> _unitTextByType = new Dictionary<UnitType, string>()
  {
    { UnitType.PAWN, "P" },
    { UnitType.BISHOP, "B" },
    { UnitType.KNIGHT, "N" },
    { UnitType.KING, "K" },
    { UnitType.QUEEN, "Q" },
    { UnitType.ROOK, "R" },
  };

  public void Init(Vector2Int pos, UnitType unit, PlayerType owner)
  {    
    ThisUnitType = unit;
    Owner = owner;
    Health = GlobalConstants.UnitHealthByType[unit];
    Damage = GlobalConstants.UnitDamageByType[unit];
      
    HealthBar.rectTransform.sizeDelta = new Vector2(0.32f * Health, 0.32f);

    UnitText.text = _unitTextByType[unit];

    UnitImage.color = (Owner == PlayerType.PLAYER1) ? Color.white : Color.cyan;

    _position = pos;

    if (unit == UnitType.PAWN)
    {
      UnitImage.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    else if (unit == UnitType.BISHOP || unit == UnitType.KNIGHT || unit == UnitType.ROOK)
    {
      UnitImage.rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }
    else if (unit == UnitType.KING || unit == UnitType.QUEEN)
    {
      UnitImage.rectTransform.localScale = Vector3.one;
    }
  }
}

public enum UnitType
{
  PAWN = 0,
  BISHOP,
  KNIGHT,
  ROOK,
  QUEEN,
  KING
}

public enum PlayerType
{
  PLAYER1 = 0,
  PLAYER2
}

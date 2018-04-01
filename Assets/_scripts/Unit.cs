using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unit : MonoBehaviour 
{
  public HealthIndicator HealthBar;

  public RectTransform RectTransformRef;
  public TMP_Text UnitText;
  public Image UnitImage;

  public UnitType ThisUnitType;
  public PlayerType Owner;

  public int Health = 1;
  public int Damage = 1;

  public bool MovedFirstTime = false;

  Vector2Int _position = Vector2Int.zero;
  public Vector2Int Position
  {
    set
    {
      transform.localPosition = new Vector3(value.x, value.y, 0.0f);
      _position.Set(value.x, value.y);
    }

    get { return _position; }
  }

  public Vector2Int ArrayCoordinates()
  {
    return new Vector2Int(_position.y, _position.x);
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
      
    HealthBar.Init(Health);

    UnitText.text = _unitTextByType[unit];

    switch (unit)
    {
      case UnitType.PAWN:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[5] : GameOverseer.Instance.UnitsSprites[11];
        break;

      case UnitType.BISHOP:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[2] : GameOverseer.Instance.UnitsSprites[8];
        break;

      case UnitType.KING:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[0] : GameOverseer.Instance.UnitsSprites[6];
        break;

      case UnitType.QUEEN:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[1] : GameOverseer.Instance.UnitsSprites[7];
        break;

      case UnitType.KNIGHT:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[3] : GameOverseer.Instance.UnitsSprites[9];
        break;

      case UnitType.ROOK:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[4] : GameOverseer.Instance.UnitsSprites[10];
        break;
    }

    //UnitImage.color = (Owner == PlayerType.PLAYER1) ? Color.white : Color.cyan;

    _position = pos;

    /*
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
    */
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

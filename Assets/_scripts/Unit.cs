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
  public bool IsRanged = false;

  Vector2Int _worldPosition = Vector2Int.zero;
  public Vector2Int WorldPosition
  {
    set
    {
      transform.localPosition = new Vector3(value.x, value.y, 0.0f);
      _worldPosition.Set(value.x, value.y);
    }

    get { return _worldPosition; }
  }

  public Vector2Int ArrayCoordinates()
  {
    return new Vector2Int(_worldPosition.y, _worldPosition.x);
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
      
    HealthBar.Init(this);

    UnitText.text = _unitTextByType[unit];

    switch (unit)
    {
      case UnitType.PAWN:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[5] : GameOverseer.Instance.UnitsSprites[11];
        IsRanged = false;
        break;

      case UnitType.BISHOP:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[2] : GameOverseer.Instance.UnitsSprites[8];
        IsRanged = true;
        break;

      case UnitType.KING:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[0] : GameOverseer.Instance.UnitsSprites[6];
        IsRanged = false;
        break;

      case UnitType.QUEEN:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[1] : GameOverseer.Instance.UnitsSprites[7];
        IsRanged = false;
        break;

      case UnitType.KNIGHT:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[3] : GameOverseer.Instance.UnitsSprites[9];
        IsRanged = false;
        break;

      case UnitType.ROOK:
        UnitImage.sprite = (Owner == PlayerType.PLAYER1) ? GameOverseer.Instance.UnitsSprites[4] : GameOverseer.Instance.UnitsSprites[10];
        IsRanged = true;
        break;
    }

    _worldPosition = pos;
  }

  public bool IsKilled = false;

  public void ReceiveDamage(int damage)
  {
    Health -= damage;

    if (Health < 0) Health = 0;

    HealthBar.UpdateHealthIndicator();

    if (Health == 0)
    {
      IsKilled = true;
      Destroy(gameObject);
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

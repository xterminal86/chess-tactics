using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour 
{
  public Canvas CanvasReference;

  public RectTransform GridHolder;
  public GameObject CellPrefab;
  public GameObject UnitPrefab;

  int _size = 8;

  Cell[,] _board;
  public Cell[,] Board
  {
    get { return _board; }
  }

	void Start () 
  {
    _board = new Cell[_size, _size];

    List<Color> colors = new List<Color>();

    colors.Add(Color.white);
    colors.Add(new Color(0.2f, 0.2f, 0.2f));

    int colorIndex = 0;

    bool flipFlag = true;

    for (int x = 0; x < _size; x++)
    {
      flipFlag = !flipFlag;

      for (int y = 0; y < _size; y++)
      {
        colorIndex = flipFlag ? 0 : 1;

        var go = Instantiate(CellPrefab);

        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(GridHolder, false);
        rt.localPosition = new Vector3(y, x, 0.0f);

        Cell c = go.GetComponent<Cell>();
        c.ImageComponent.color = colors[colorIndex];

        _board[x, y] = c;

        flipFlag = !flipFlag;
      }
    }

    SetupPlayer1();
    SetupPlayer2();
	}

  void SetupPlayer1()
  {
    for (int x = 0; x < _size; x++)
    {
      PlaceUnit(new Vector2Int(x, 1), UnitType.PAWN, PlayerType.PLAYER1);
    }

    PlaceUnit(new Vector2Int(4, 0), UnitType.KING, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(3, 0), UnitType.QUEEN, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(1, 0), UnitType.KNIGHT, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(6, 0), UnitType.KNIGHT, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 0), UnitType.ROOK, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(7, 0), UnitType.ROOK, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(2, 0), UnitType.BISHOP, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(5, 0), UnitType.BISHOP, PlayerType.PLAYER1);
  }

  void SetupPlayer2()
  {
    for (int x = 0; x < _size; x++)
    {
      PlaceUnit(new Vector2Int(x, _size - 2), UnitType.PAWN, PlayerType.PLAYER2);
    }

    PlaceUnit(new Vector2Int(4, _size - 1), UnitType.KING, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(3, _size - 1), UnitType.QUEEN, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(1, _size - 1), UnitType.KNIGHT, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(6, _size - 1), UnitType.KNIGHT, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(0, _size - 1), UnitType.ROOK, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(7, _size - 1), UnitType.ROOK, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(2, _size - 1), UnitType.BISHOP, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(5, _size - 1), UnitType.BISHOP, PlayerType.PLAYER2);
  }

  void PlaceUnit(Vector2Int pos, UnitType type, PlayerType owner)
  {
    var go = Instantiate(UnitPrefab);

    var rt = go.GetComponent<RectTransform>();
    rt.SetParent(CanvasReference.transform, false);
    rt.localPosition = new Vector3(pos.x, pos.y, 0.0f);

    Unit u = go.GetComponent<Unit>();
    u.Init(pos, type, owner, GlobalConstants.UnitHealthByType[type]);
  }
}

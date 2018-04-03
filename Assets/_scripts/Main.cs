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
  public GameObject SelectionOutlinePrefab;

  int _size = 8;

  Cell[,] _board;
  public Cell[,] Board
  {
    get { return _board; }
  }

  GameObject _selectionOutline;

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
        c.ArrayCoordinates.Set(x, y);
        c.ImageComponent.color = colors[colorIndex];

        _board[x, y] = c;

        flipFlag = !flipFlag;
      }
    }

    SetupPlayer1();
    SetupPlayer2();

    _selectionOutline = Instantiate(SelectionOutlinePrefab);
    _selectionOutline.GetComponent<RectTransform>().SetParent(GridHolder, false);
	}

  void SetupPlayer1()
  {
    for (int y = 0; y < _size; y++)
    {
      PlaceUnit(new Vector2Int(1, y), UnitType.PAWN, PlayerType.PLAYER1);
    }

    PlaceUnit(new Vector2Int(0, 4), UnitType.KING, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 3), UnitType.QUEEN, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 1), UnitType.KNIGHT, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 6), UnitType.KNIGHT, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 0), UnitType.ROOK, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 7), UnitType.ROOK, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 2), UnitType.BISHOP, PlayerType.PLAYER1);
    PlaceUnit(new Vector2Int(0, 5), UnitType.BISHOP, PlayerType.PLAYER1);
  }

  void SetupPlayer2()
  {
    for (int y = 0; y < _size; y++)
    {
      PlaceUnit(new Vector2Int(_size - 2, y), UnitType.PAWN, PlayerType.PLAYER2);
    }

    PlaceUnit(new Vector2Int(_size - 1, 4), UnitType.KING, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(_size - 1, 3), UnitType.QUEEN, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(_size - 1, 1), UnitType.KNIGHT, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(_size - 1, 6), UnitType.KNIGHT, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(_size - 1, 0), UnitType.ROOK, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(_size - 1, 7), UnitType.ROOK, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(_size - 1, 2), UnitType.BISHOP, PlayerType.PLAYER2);
    PlaceUnit(new Vector2Int(_size - 1, 5), UnitType.BISHOP, PlayerType.PLAYER2);

    // Test
    //PlaceUnit(new Vector2Int(3, 2), UnitType.PAWN, PlayerType.PLAYER2);
  }

  void PlaceUnit(Vector2Int pos, UnitType type, PlayerType owner)
  {
    var go = Instantiate(UnitPrefab);

    var rt = go.GetComponent<RectTransform>();
    rt.SetParent(CanvasReference.transform, false);
    rt.localPosition = new Vector3(pos.y, pos.x, 0.0f);

    Unit u = go.GetComponent<Unit>();
    u.Init(new Vector2Int(pos.y, pos.x), type, owner);

    _board[pos.x, pos.y].UnitPresent = u;
  }

  RaycastHit _hitInfo;
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(r.origin, r.direction, out _hitInfo, Mathf.Infinity))
      {
        Cell c = _hitInfo.collider.gameObject.GetComponent<Cell>();

        int cx = c.ArrayCoordinates.x;
        int cy = c.ArrayCoordinates.y;

        ResetCellColors();

        if (c.UnitPresent != null && c.UnitPresent.Owner == GameOverseer.Instance.PlayerTurn)
        {
          _selectionOutline.transform.localPosition = new Vector3(c.UnitPresent.WorldPosition.x, c.UnitPresent.WorldPosition.y, 0.0f);
          _selectionOutline.gameObject.SetActive(true);

          ShowValidMoves(c.UnitPresent);

          GameOverseer.Instance.SelectedUnit = _board[cx, cy].UnitPresent;
        }
        else
        {
          TryToMoveUnit(c);
          _selectionOutline.gameObject.SetActive(false);
        }
      }
    }
    else if (Input.GetMouseButtonDown(1))
    {
      ResetCellColors();
      _selectionOutline.gameObject.SetActive(false);
    }
  }

  void TryToMoveUnit(Cell c)
  {
    if (GameOverseer.Instance.SelectedUnit != null)
    {
      int cx = c.ArrayCoordinates.x;
      int cy = c.ArrayCoordinates.y;

      int x = 0;
      int y = 0;

      foreach (var item in _validMoveCells)
      {        
        x = item.ArrayCoordinates.x;
        y = item.ArrayCoordinates.y;

        if (cx == x && cy == y)
        {
          if (GlobalConstants.CommandPointsByUnit[GameOverseer.Instance.SelectedUnit.ThisUnitType] > GameOverseer.Instance.CommandPoints)
          {
            Debug.LogWarning("Not enough command points!");
            return;
          }

          Vector2Int oldCoords = GameOverseer.Instance.SelectedUnit.ArrayCoordinates();
          GameOverseer.Instance.SelectedUnit.WorldPosition = new Vector2Int(y, x);

          if (!GameOverseer.Instance.SelectedUnit.MovedFirstTime)
          {
            GameOverseer.Instance.SelectedUnit.MovedFirstTime = true;
          }
            
          _board[x, y].UnitPresent = GameOverseer.Instance.SelectedUnit;
          _board[oldCoords.x, oldCoords.y].UnitPresent = null;

          GameOverseer.Instance.SpendCommandPoints(GlobalConstants.CommandPointsByUnit[GameOverseer.Instance.SelectedUnit.ThisUnitType]);

          if (GameOverseer.Instance.CommandPoints == 0)
          {
            GameOverseer.Instance.TurnDone();
          }

          break;
        }
      }

      GameOverseer.Instance.SelectedUnit = null;
    }
  }

  List<Cell> _validMoveCells = new List<Cell>();

  Color _transparentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
  Color _validColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
  Color _invalidColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
  void ShowValidMoves(Unit unit)
  {    
    _validMoveCells.Clear();

    int ux = unit.ArrayCoordinates().x;
    int uy = unit.ArrayCoordinates().y;

    int hx = ux + 1;
    int hy = uy + 1;
    int lx = ux - 1;
    int ly = uy - 1;

    switch (unit.ThisUnitType)
    {
      case UnitType.PAWN:
        if (unit.Owner == PlayerType.PLAYER1)
        {
          for (int i = 1; i < 3; i++)
          {
            if (unit.MovedFirstTime && i > 1)
            {
              break;
            }

            if (LimitsValid(lx, ly, hx, hy) && (_board[hx, hy].UnitPresent && _board[hx, hy].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn))
            {
              _board[hx, hy].IsValidIndicator.color = _invalidColor;
              _validMoveCells.Add(_board[hx, hy]);
            }

            if (LimitsValid(lx, ly, hx, hy) && (_board[hx, ly].UnitPresent && _board[hx, ly].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn))
            {
              _board[hx, ly].IsValidIndicator.color = _invalidColor;
              _validMoveCells.Add(_board[hx, ly]);
            }

            if (_board[ux + i, uy].UnitPresent)
            {
              break;            
            }

            _validMoveCells.Add(_board[ux + i, uy]);

            _board[ux + i, uy].IsValidIndicator.color = _validColor;
          }
        }
        else
        {
          for (int i = 1; i < 3; i++)
          {
            if (unit.MovedFirstTime && i > 1)
            {
              break;
            }

            if (LimitsValid(lx, ly, hx, hy) && (_board[lx, ly].UnitPresent && _board[lx, ly].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn))
            {
              _board[lx, ly].IsValidIndicator.color = _invalidColor;
              _validMoveCells.Add(_board[lx, ly]);
            }

            if (LimitsValid(lx, ly, hx, hy) && (_board[lx, hy].UnitPresent && _board[lx, hy].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn))
            {
              _board[lx, hy].IsValidIndicator.color = _invalidColor;
              _validMoveCells.Add(_board[lx, hy]);
            }

            if (_board[ux - i, uy].UnitPresent)
            {
              break;            
            }

            _validMoveCells.Add(_board[ux - i, uy]);

            _board[ux - i, uy].IsValidIndicator.color = _validColor;
          }
        }

        break;
    }
  }

  bool LimitsValid(int lx, int ly, int hx, int hy)
  {
    return (lx >= 0 && hx < _size && lx >= 0 && hy < _size);
  }

  void ResetCellColors()
  {
    for (int x = 0; x < _size; x++)
    {
      for (int y = 0; y < _size; y++)
      {
        _board[x, y].IsValidIndicator.color = _transparentColor;
      }
    }
  }
}

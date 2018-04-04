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

  Unit _selectedUnit;

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

          _selectedUnit = _board[cx, cy].UnitPresent;
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
    if (_selectedUnit != null)
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
          if (GlobalConstants.CommandPointsByUnit[_selectedUnit.ThisUnitType] > GameOverseer.Instance.CommandPoints)
          {
            Debug.LogWarning("Not enough command points!");
            return;
          }

          Unit unitPresent = _board[x, y].UnitPresent;

          if (!unitPresent)
          {
            MoveUnit(_selectedUnit, new Vector2Int(x, y));
            GameOverseer.Instance.SpendCommandPoints(GlobalConstants.CommandPointsByUnit[_selectedUnit.ThisUnitType]);
            break;
          }
          else if (unitPresent && unitPresent.Owner != GameOverseer.Instance.PlayerTurn)
          {
            if (!_selectedUnit.IsRanged)
            {
              unitPresent.ReceiveDamage(GlobalConstants.UnitDamageByType[_selectedUnit.ThisUnitType]);

              if (unitPresent.IsKilled)
              {
                MoveUnit(_selectedUnit, unitPresent.ArrayCoordinates());
              }

              GameOverseer.Instance.SpendCommandPoints(GlobalConstants.CommandPointsByUnit[_selectedUnit.ThisUnitType]);
            }
            else
            {
              unitPresent.ReceiveDamage(GlobalConstants.UnitDamageByType[_selectedUnit.ThisUnitType]);
              GameOverseer.Instance.SpendCommandPoints(GlobalConstants.CommandPointsByUnit[_selectedUnit.ThisUnitType]);
            }
          }
        }
      }

      _selectedUnit = null;
    }
  }

  void MoveUnit(Unit unit, Vector2Int coords)
  {
    Vector2Int oldCoords = unit.ArrayCoordinates();
    unit.WorldPosition = new Vector2Int(coords.y, coords.x);

    if (!unit.MovedFirstTime)
    {
      unit.MovedFirstTime = true;
    }

    _board[coords.x, coords.y].UnitPresent = unit;
    _board[oldCoords.x, oldCoords.y].UnitPresent = null;
  }

  List<Cell> _validMoveCells = new List<Cell>();

  Color _transparentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
  Color _validColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
  Color _invalidColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
  void ShowValidMoves(Unit unit)
  {    
    _validMoveCells.Clear();

    switch (unit.ThisUnitType)
    {
      case UnitType.PAWN:
        ProcessPawn(unit);
        break;

      case UnitType.KNIGHT:
        ProcessKnight(unit);
        break;

      case UnitType.BISHOP:
        ProcessBishop(unit);
        break;

      case UnitType.ROOK:
        ProcessRook(unit);
        break;

      case UnitType.QUEEN:
      case UnitType.KING:
        ProcessKingAndQueen(unit);
        break;        
    }
  }

  // FIXME: 1 cell movement / attack for now for both
  void ProcessKingAndQueen(Unit unit)
  {
    int ux = unit.ArrayCoordinates().x;
    int uy = unit.ArrayCoordinates().y;

    int lx = ux - 1;
    int ly = uy - 1;
    int hx = ux + 1;
    int hy = uy + 1;

    for (int x = lx; x <= hx; x++)
    {
      for (int y = ly; y <= hy; y++)
      {
        Vector2Int coord = new Vector2Int(x, y);

        if (!IsInGrid(coord) || (x == ux && y == uy))
        {
          continue;
        }

        if (_board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn)
        {
          _board[coord.x, coord.y].IsValidIndicator.color = _invalidColor;
          _validMoveCells.Add(_board[coord.x, coord.y]);
        }
        else if (!_board[coord.x, coord.y].UnitPresent)
        {
          _board[coord.x, coord.y].IsValidIndicator.color = _validColor;
          _validMoveCells.Add(_board[coord.x, coord.y]);
        }
        else if (_board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner == GameOverseer.Instance.PlayerTurn)
        {
          continue;
        }          
      }
    }
  }

  void ProcessRook(Unit unit)
  {
    int ux = unit.ArrayCoordinates().x;
    int uy = unit.ArrayCoordinates().y;

    List<Vector2Int> mults = new List<Vector2Int>() 
    {
      new Vector2Int(1, 0),
      new Vector2Int(0, 1),
      new Vector2Int(-1, 0),
      new Vector2Int(0, -1)
    };

    for (int i = 0; i < 4; i++)
    {
      for (int additive = 1; additive < _size; additive++)
      {
        Vector2Int coord = new Vector2Int(ux + additive * mults[i].x, uy + additive * mults[i].y);

        if (!IsInGrid(coord))
        {
          break;
        }

        if (_board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn)
        {
          _board[coord.x, coord.y].IsValidIndicator.color = _invalidColor;
          _validMoveCells.Add(_board[coord.x, coord.y]);
          break;
        }

        if (_board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner == GameOverseer.Instance.PlayerTurn)
        {
          break;
        }

        if (!_board[coord.x, coord.y].UnitPresent)
        {
          _board[coord.x, coord.y].IsValidIndicator.color = _validColor;
          _validMoveCells.Add(_board[coord.x, coord.y]);
        }
      }
    }
  }

  void ProcessBishop(Unit unit)
  {
    int ux = unit.ArrayCoordinates().x;
    int uy = unit.ArrayCoordinates().y;

    List<Vector2Int> mults = new List<Vector2Int>() 
    {
      new Vector2Int(1, 1),
      new Vector2Int(1, -1),
      new Vector2Int(-1, 1),
      new Vector2Int(-1, -1)
    };

    for (int i = 0; i < 4; i++)
    {
      for (int additive = 1; additive < _size; additive++)
      {
        Vector2Int coord = new Vector2Int(ux + additive * mults[i].x, uy + additive * mults[i].y);

        if (!IsInGrid(coord))
        {
          break;
        }

        if (_board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn)
        {
          _board[coord.x, coord.y].IsValidIndicator.color = _invalidColor;
          _validMoveCells.Add(_board[coord.x, coord.y]);
          break;
        }

        if (_board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner == GameOverseer.Instance.PlayerTurn)
        {
          break;
        }

        if (!_board[coord.x, coord.y].UnitPresent)
        {
          _board[coord.x, coord.y].IsValidIndicator.color = _validColor;
          _validMoveCells.Add(_board[coord.x, coord.y]);
        }
      }
    }
  }

  void ProcessKnight(Unit unit)
  {
    int ux = unit.ArrayCoordinates().x;
    int uy = unit.ArrayCoordinates().y;

    List<Vector2Int> validCells = new List<Vector2Int>() 
    {
      new Vector2Int(ux + 2, uy + 1),
      new Vector2Int(ux + 1, uy + 2),
      new Vector2Int(ux - 1, uy + 2),
      new Vector2Int(ux - 2, uy + 1),
      new Vector2Int(ux - 2, uy - 1),
      new Vector2Int(ux - 1, uy - 2),
      new Vector2Int(ux + 1, uy - 2),
      new Vector2Int(ux + 2, uy - 1)
    };

    foreach (var coord in validCells)
    {
      if (IsInGrid(coord) && _board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn)
      {
        _board[coord.x, coord.y].IsValidIndicator.color = _invalidColor;
        _validMoveCells.Add(_board[coord.x, coord.y]);
      }
      else if (IsInGrid(coord) && _board[coord.x, coord.y].UnitPresent && _board[coord.x, coord.y].UnitPresent.Owner == GameOverseer.Instance.PlayerTurn)
      {
        continue;
      }
      else if (IsInGrid(coord) && !_board[coord.x, coord.y].UnitPresent)
      {
        _board[coord.x, coord.y].IsValidIndicator.color = _validColor;
        _validMoveCells.Add(_board[coord.x, coord.y]);
      }
    }
  }

  void ProcessPawn(Unit unit)
  {
    int ux = unit.ArrayCoordinates().x;
    int uy = unit.ArrayCoordinates().y;

    if (unit.Owner == PlayerType.PLAYER1)
    {
      List<Vector2Int> diagonals1 = new List<Vector2Int>() 
      {
        new Vector2Int(ux + 1, uy + 1),
        new Vector2Int(ux + 1, uy - 1)
      };

      Vector2Int oneStep1 = new Vector2Int(ux + 1, uy);
      Vector2Int twoSteps1 = new Vector2Int(ux + 2, uy);

      CheckPawnMovement(unit, diagonals1, oneStep1, twoSteps1);
    }
    else
    {
      List<Vector2Int> diagonals2 = new List<Vector2Int>() 
      {
        new Vector2Int(ux - 1, uy + 1),
        new Vector2Int(ux - 1, uy - 1)
      };

      Vector2Int oneStep2 = new Vector2Int(ux - 1, uy);
      Vector2Int twoSteps2 = new Vector2Int(ux - 2, uy);

      CheckPawnMovement(unit, diagonals2, oneStep2, twoSteps2);
    }
  }

  void CheckPawnMovement(Unit unit, List<Vector2Int> diagonals, Vector2Int oneStep, Vector2Int twoSteps)
  {
    foreach (var item in diagonals)
    {
      if (IsInGrid(item) && _board[item.x, item.y].UnitPresent && _board[item.x, item.y].UnitPresent.Owner != GameOverseer.Instance.PlayerTurn)
      {
        _board[item.x, item.y].IsValidIndicator.color = _invalidColor;
        _validMoveCells.Add(_board[item.x, item.y]);
      }
    }

    if (IsInGrid(oneStep) && _board[oneStep.x, oneStep.y].UnitPresent)
    {
      return;
    }
    else if (IsInGrid(oneStep) && !_board[oneStep.x, oneStep.y].UnitPresent)
    {
      _board[oneStep.x, oneStep.y].IsValidIndicator.color = _validColor;
      _validMoveCells.Add(_board[oneStep.x, oneStep.y]);
    }

    if (!unit.MovedFirstTime)
    {
      if (IsInGrid(twoSteps) && _board[twoSteps.x, twoSteps.y].UnitPresent)
      {
        return;
      }
      else if (IsInGrid(twoSteps) && !_board[twoSteps.x, twoSteps.y].UnitPresent)
      {
        _board[twoSteps.x, twoSteps.y].IsValidIndicator.color = _validColor;
        _validMoveCells.Add(_board[twoSteps.x, twoSteps.y]);
      }
    }
  }

  bool IsInGrid(Vector2Int coord)
  {
    return (coord.x >= 0 && coord.x < _size && coord.y >= 0 && coord.y < _size);
  }

  bool LimitsValid(int lx, int ly, int hx, int hy)
  {
    return (lx >= 0 && hx < _size && ly >= 0 && hy < _size);
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

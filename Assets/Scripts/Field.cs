using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public static Field Instance;

    [SerializeField] private float _cellSize;
    [SerializeField] private float _cellSpacing;
    [SerializeField] private uint _fieldSize;

    [SerializeField] private uint _initCellsCount;

    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private RectTransform _rectTransform;

    private Cell[,] _field;

    private bool _anyCellMoved;

    private void Awake()
    {
        if (Instance is null) 
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            OnInput(Vector2.left);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            OnInput(Vector2.up);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            OnInput(Vector2.right);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            OnInput(Vector2.down);

    }

    private void OnInput(Vector2 direction)
    {
        if (!Root.GameStarted) return;
        
        _anyCellMoved = false;
        ResetCellsFlags();

        NextMove(direction);

        if (_anyCellMoved)
        {
            GenerateRandomCell();
            CheckGameResult();
        }

    }

    private void NextMove(Vector2 direction)
    {
        int startXY = direction.x > 0 || direction.y < 0 ? (int)_fieldSize - 1 : 0;
        int dir = direction.x != 0 ? (int)direction.x : -(int)direction.y;

        for (int i = 0; i < _fieldSize; i++)
        {
            for (int j = startXY; j >= 0 && j < _fieldSize; j -= dir)
            {
                Cell cell = direction.x != 0 ? _field[j, i] : _field[i, j];

                if (cell.IsEmpty)
                    continue;

                Cell cellToMerge = FindCellToMerge(cell, direction);
                if (cellToMerge is not null) 
                {
                    cell.Merge(cellToMerge);
                    _anyCellMoved = true;

                    continue;
                }

                Cell emptyCell = FindEmptyCell(cell, direction);
                if (emptyCell is not null)
                {
                    cell.MoveTo(emptyCell);
                    _anyCellMoved = true;

                    continue;
                }

            }
        }
    }

    private Cell FindCellToMerge(Cell cell, Vector2 direction)
    {
        int startX = (int)(cell.X + direction.x);
        int startY = (int)(cell.Y - direction.y);

        for (int x = startX, y = startY; 
            x >= 0 && x < _fieldSize && y >= 0 && y < _fieldSize;
            x += (int)direction.x, y -= (int)direction.y)
        {
            if (_field[x, y].IsEmpty)
                continue;

            if (_field[x, y].Value == cell.Value && !_field[x, y].HasMerged)
                return _field[x, y];

            break;
        }

        return null;
    }

    private Cell FindEmptyCell(Cell cell, Vector2 direction) 
    {
        Cell emptyCell = null;

        int startX = (int)(cell.X + direction.x);
        int startY = (int)(cell.Y - direction.y);

        for (int x = startX, y = startY;
            x >= 0 && x < _fieldSize && y >= 0 && y < _fieldSize;
            x += (int)direction.x, y -= (int)direction.y)
        {
            if (_field[x, y].IsEmpty)
                emptyCell = _field[x, y];
            else
                break;
        }

        return emptyCell;
    }

    private void CheckGameResult()
    {
        bool lose = true;

        for (uint y = 0; y < _fieldSize; y++)
        {
            for (uint x = 0; x < _fieldSize; x++)
            {
                if (_field[x, y].Value == Cell.MaxValue)
                {
                    Root.Instance.Win();
                    return;
                }

                if (lose && _field[x, y].IsEmpty ||
                    FindCellToMerge(_field[x, y], Vector2.left) ||
                    FindCellToMerge(_field[x, y], Vector2.right) ||
                    FindCellToMerge(_field[x, y], Vector2.up) ||
                    FindCellToMerge(_field[x, y], Vector2.down))
                {
                    lose = false;
                }
            }
        }

        if (lose) 
            Root.Instance.Lose();
    }

    private void CreateField()
    {
        _field = new Cell[_fieldSize, _fieldSize];

        float fieldWidth = _fieldSize * (_cellSize + _cellSpacing) + _cellSpacing;
        _rectTransform.sizeDelta = new Vector2(fieldWidth, fieldWidth);

        float startX = -(fieldWidth / 2) + (_cellSize / 2) + _cellSpacing;
        float startY = (fieldWidth / 2) - (_cellSize / 2) - _cellSpacing;

        for (uint y = 0; y < _fieldSize; y++)
        {
            for (uint x = 0; x < _fieldSize; x++)
            {
                Cell cell = Instantiate(_cellPrefab, transform, false);

                float cellX = startX + (x * (_cellSize + _cellSpacing));
                float cellY = startY - (y * (_cellSize + _cellSpacing));
                
                Vector2 position = new Vector2(cellX, cellY);

                cell.transform.localPosition = position;

                _field[y, x] = cell;

                cell.SetCell(x, y, 0);
            }
        }
    }

    private void GenerateRandomCell()
    {
        var emptyCells = new List<Cell>();

        for (uint y = 0; y < _fieldSize; y++)
            for (uint x = 0; x < _fieldSize; x++)
                if (_field[x, y].IsEmpty)
                    emptyCells.Add(_field[x, y]);

        if (emptyCells.Count == 0) 
            throw new System.Exception("There is no any empty cell!");

        uint value = Random.Range(0, 10) == 0 ? (uint)2 : 1;
        
        Cell cell = emptyCells[Random.Range(0, emptyCells.Count)];
        cell.SetCell(cell.X, cell.Y, value, false);

        CellAnimator.Instance.SmoothAppear(cell);
    }

    private void ResetCellsFlags()
    {
        for (uint y = 0; y < _fieldSize; y++)
            for (uint x = 0; x < _fieldSize; x++)
                _field[x, y].ResetFlags();
    }

    public void GenerateField()
    {
        if (_field is null)
            CreateField();

        for (uint y = 0; y < _fieldSize; y++)
            for (uint x = 0; x < _fieldSize; x++)
                _field[x, y].SetCell(x, y, 0);

        for (int i = 0; i < _initCellsCount; i++)
            GenerateRandomCell();
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts
{
    public class MapGenerator : StaticInstance<MapGenerator>
    {
        [FormerlySerializedAs("width")]
        [Header("Map Size")]
        [SerializeField] public int Width = 20;
        [SerializeField] public int Height = 10;

        [Header("Factory")]
        [SerializeField] private CellFactory cellFactory;

        [Header("Parents")]
        [SerializeField] private Transform cellsParent;
        [SerializeField] private Transform wallsParent;
        
        private const int OFFSET = 3;
        public Vector2 MinBounds => new Vector2(- OFFSET, - OFFSET);
        public Vector2 MaxBounds => new Vector2(Width + OFFSET, Height + OFFSET);
        
        public IReadOnlyList<Cell> WalkableCells => _walkableCells;
        public IReadOnlyList<Cell> WallCells => _wallCells;

        private readonly List<Cell> _walkableCells = new();
        private readonly List<Cell> _wallCells = new();
        
        public void Generate()
        {
            Clear();

            GenerateMainField();
            GenerateBottomWall();
        }

        private void GenerateMainField()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    CellType type = ResolveCellType(x, y);
                    Cell cell = cellFactory.Create(type, x, y, cellsParent);

                    _walkableCells.Add(cell);
                }
            }
        }

        private void GenerateBottomWall()
        {
            int wallY = -1;

            for (int x = 0; x < Width; x++)
            {
                CellType type;

                if (x == 0)
                    type = CellType.LeftCornerWall;
                else if (x == Width - 1)
                    type = CellType.RightCornerWall;
                else
                    type = CellType.BottomCornerWall;

                Cell wall = cellFactory.Create(type, x, wallY, wallsParent);
                _wallCells.Add(wall);
            }
        }

        private CellType ResolveCellType(int x, int y)
        {
            bool left = x == 0;
            bool right = x == Width - 1;
            bool bottom = y == 0;
            bool top = y == Height - 1;

            if (left && bottom) return CellType.LowerLeftCorner;
            if (right && bottom) return CellType.LowerRightCorner;
            if (left && top) return CellType.UpperLeftCorner;
            if (right && top) return CellType.UpperRightCorner;

            if (bottom) return CellType.BottomCorner;
            if (top) return CellType.UpperCorner;
            if (left) return CellType.LeftCorner;
            if (right) return CellType.RightCorner;

            return CellType.Default;
        }

        private void Clear()
        {
            foreach (var cell in _walkableCells)
                if (cell) Destroy(cell.gameObject);

            foreach (var wall in _wallCells)
                if (wall) Destroy(wall.gameObject);

            _walkableCells.Clear();
            _wallCells.Clear();
        }
    }
}

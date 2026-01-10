using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class TreesGenerator : Singleton<TreesGenerator>
    {
        [Header("Factory")]
        [SerializeField] private CellFactory cellFactory;

        [Header("Parents")]
        [SerializeField] private Transform cellsParent;
        
        [Header("Generation Settings")]
        [SerializeField] private float treeDensity = 0.2f;
        [SerializeField] private float forestThickness = 3f;
        [SerializeField] private float minDistanceBetweenTrees = 1.5f;
        [SerializeField] private ForestGenerationStrategy strategy = ForestGenerationStrategy.EdgeRing;
        
        public IReadOnlyList<Cell> Trees => _trees;

        private readonly List<Cell> _trees = new();
        private readonly List<Vector2Int> _treePositions = new();
        
        private System.Random _random;
        
        public List<Cell> GetEmptyCells()
        {
            IReadOnlyList<Cell> allWalkableCells = MapGenerator.Instance.WalkableCells;
            HashSet<Cell> roadCells = RoadGenerator.Instance.RoadCells;
            List<Cell> emptyCells = new List<Cell>();
            
            foreach (Cell walkableCell in allWalkableCells)
            {
                bool isRoadCell = false;
                foreach (Cell roadCell in roadCells)
                {
                    if (walkableCell.X == roadCell.X && walkableCell.Y == roadCell.Y)
                    {
                        isRoadCell = true;
                        break;
                    }
                }
                if (!isRoadCell) emptyCells.Add(walkableCell);
            }
            
            return emptyCells;
        }
        
        public void Generate(int seed = 0)
        {
            Clear();
            
            _random = seed == 0 ? new System.Random() : new System.Random(seed);
            _treePositions.Clear();
            
            List<Cell> emptyCells = GetEmptyCells();
            if (emptyCells.Count == 0) return;
            
            switch (strategy)
            {
                case ForestGenerationStrategy.EdgeRing:
                    GenerateEdgeRing(emptyCells);
                    break;
                case ForestGenerationStrategy.SphereRing:
                    GenerateSphereRing(emptyCells);
                    break;
                case ForestGenerationStrategy.Clusters:
                    GenerateClusters(emptyCells);
                    break;
                case ForestGenerationStrategy.RandomScatter:
                    GenerateRandomScatter(emptyCells);
                    break;
            }
        }
        
        private void GenerateEdgeRing(List<Cell> emptyCells)
        {
            IReadOnlyList<Cell> allCells = MapGenerator.Instance.WalkableCells;
            if (allCells.Count == 0) return;
            
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            
            foreach (Cell cell in allCells)
            {
                if (cell.X < minX) minX = cell.X;
                if (cell.X > maxX) maxX = cell.X;
                if (cell.Y < minY) minY = cell.Y;
                if (cell.Y > maxY) maxY = cell.Y;
            }
            
            foreach (Cell cell in emptyCells)
            {
                if (cell.X <= minX + forestThickness || 
                    cell.X >= maxX - forestThickness ||
                    cell.Y <= minY + forestThickness || 
                    cell.Y >= maxY - forestThickness)
                {
                    if (_random.NextDouble() <= treeDensity)
                    {
                        Vector2Int pos = new Vector2Int(cell.X, cell.Y);
                        if (!IsTooClose(pos))
                        {
                            CreateTreeAtPosition(pos);
                        }
                    }
                }
            }
        }
        
        private void GenerateSphereRing(List<Cell> emptyCells)
        {
            IReadOnlyList<Cell> allCells = MapGenerator.Instance.WalkableCells;
            if (allCells.Count == 0) return;
            
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            
            foreach (Cell cell in allCells)
            {
                if (cell.X < minX) minX = cell.X;
                if (cell.X > maxX) maxX = cell.X;
                if (cell.Y < minY) minY = cell.Y;
                if (cell.Y > maxY) maxY = cell.Y;
            }
            
            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            float radius = Mathf.Min(maxX - minX, maxY - minY) / 2f;
            float innerRadius = radius * 0.7f;
            float outerRadius = radius * 0.95f;
            
            foreach (Cell cell in emptyCells)
            {
                float distance = Vector2.Distance(
                    new Vector2(cell.X, cell.Y), 
                    new Vector2(centerX, centerY)
                );
                
                if (distance >= innerRadius && distance <= outerRadius)
                {
                    float normalized = (distance - innerRadius) / (outerRadius - innerRadius);
                    float probability = Mathf.Sin(normalized * Mathf.PI);
                    probability *= (0.8f + (float)_random.NextDouble() * 0.4f);
                    
                    if (_random.NextDouble() <= probability * treeDensity)
                    {
                        Vector2Int pos = new Vector2Int(cell.X, cell.Y);
                        if (!IsTooClose(pos))
                        {
                            CreateTreeAtPosition(pos);
                        }
                    }
                }
            }
        }
        
        private void GenerateClusters(List<Cell> emptyCells)
        {
            IReadOnlyList<Cell> allCells = MapGenerator.Instance.WalkableCells;
            if (allCells.Count == 0) return;
            
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            
            foreach (Cell cell in allCells)
            {
                if (cell.X < minX) minX = cell.X;
                if (cell.X > maxX) maxX = cell.X;
                if (cell.Y < minY) minY = cell.Y;
                if (cell.Y > maxY) maxY = cell.Y;
            }
            
            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            float radius = Mathf.Min(maxX - minX, maxY - minY) / 2f;
            
            int clusterCount = Mathf.Max(4, (int)(radius / 3f));
            
            for (int i = 0; i < clusterCount; i++)
            {
                float angle = (float)i / clusterCount * Mathf.PI * 2f;
                float clusterRadius = radius * 0.85f;
                Vector2 clusterCenter = new Vector2(
                    centerX + Mathf.Cos(angle) * clusterRadius,
                    centerY + Mathf.Sin(angle) * clusterRadius
                );
                
                int clusterSize = _random.Next(3, 8);
                List<Cell> clusterCells = new List<Cell>();
                
                foreach (Cell cell in emptyCells)
                {
                    float distance = Vector2.Distance(clusterCenter, new Vector2(cell.X, cell.Y));
                    if (distance <= clusterSize)
                    {
                        clusterCells.Add(cell);
                    }
                }
                
                int treesInCluster = Mathf.Min(clusterSize * 2, clusterCells.Count);
                for (int j = 0; j < treesInCluster; j++)
                {
                    if (j >= clusterCells.Count) break;
                    
                    Cell cell = clusterCells[_random.Next(clusterCells.Count)];
                    Vector2Int pos = new Vector2Int(cell.X, cell.Y);
                    
                    if (!_treePositions.Contains(pos))
                    {
                        if (!IsTooClose(pos, 1f))
                        {
                            CreateTreeAtPosition(pos);
                        }
                    }
                }
            }
        }
        
        private void GenerateRandomScatter(List<Cell> emptyCells)
        {
            foreach (Cell cell in emptyCells)
            {
                if (_random.NextDouble() <= treeDensity)
                {
                    Vector2Int pos = new Vector2Int(cell.X, cell.Y);
                    if (!IsTooClose(pos))
                    {
                        CreateTreeAtPosition(pos);
                    }
                }
            }
        }
        
        private bool IsTooClose(Vector2Int position, float minDistance = -1)
        {
            if (minDistance < 0) minDistance = minDistanceBetweenTrees;
            
            foreach (Vector2Int treePos in _treePositions)
            {
                float distance = Vector2Int.Distance(position, treePos);
                if (distance < minDistance)
                {
                    return true;
                }
            }
            return false;
        }
        
        private void CreateTreeAtPosition(Vector2Int position)
        {
            CellType treeType = GetRandomTreeType();
            Cell treeCell = cellFactory.Create(treeType, position.x, position.y, cellsParent);
            
            _trees.Add(treeCell);
            _treePositions.Add(position);
        }
        
        private CellType GetRandomTreeType()
        {
            int typeIndex = _random.Next(4);
            return typeIndex switch
            {
                0 => CellType.ChristmasTree,
                1 => CellType.Spruce,
                2 => CellType.Birch,
                3 => CellType.Ash,
                _ => CellType.ChristmasTree
            };
        }
        
        private void Clear()
        {
            foreach (var cell in _trees)
                if (cell) Destroy(cell.gameObject);

            _trees.Clear();
            _treePositions.Clear();
        }
        
        public void SetStrategy(ForestGenerationStrategy newStrategy)
        {
            strategy = newStrategy;
        }
        
        public void SetDensity(float density)
        {
            treeDensity = Mathf.Clamp01(density);
        }
        
        public void SetThickness(float thickness)
        {
            forestThickness = Mathf.Max(1f, thickness);
        }
        
        public void SetMinDistance(float distance)
        {
            minDistanceBetweenTrees = Mathf.Max(0.5f, distance);
        }
    }
    
    public enum ForestGenerationStrategy
    {
        EdgeRing,
        SphereRing,
        Clusters,
        RandomScatter
    }
}
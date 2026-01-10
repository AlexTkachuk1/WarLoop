using System.Collections.Generic;
using _Scripts.Actors.Buildings;
using UnityEngine;

namespace _Scripts
{
    public class RoadGenerator : Singleton<RoadGenerator>
    {
        [Header("Factory")]
        [SerializeField] private CellFactory cellFactory;

        [Header("Parent for roads")]
        [SerializeField] private Transform roadsParent;

        private Dictionary<Vector2Int, Cell> _existingRoadCells = new();
        
        public HashSet<Cell> RoadCells = new();
        
        public void GenerateRoads(int minConnections = 1, int maxConnections = 3)
        {
            _existingRoadCells.Clear();
            
            var walkable = MapGenerator.Instance.WalkableCells;
            if (walkable == null || walkable.Count == 0)
                return;

            var pathfinder = new Pathfinder(walkable);

            foreach (var cell in BuildingsGenerator.Instance.Towers)
            {
                var tower = cell.GetComponent<Building>();
                if (tower == null) continue;

                tower.Roads.Clear();

                List<Cell> towers = new List<Cell>(BuildingsGenerator.Instance.Towers);
                towers.Remove(cell);

                towers.Sort((a, b) =>
                {
                    int da = Mathf.Abs(a.X - cell.X) + Mathf.Abs(a.Y - cell.Y);
                    int db = Mathf.Abs(b.X - cell.X) + Mathf.Abs(b.Y - cell.Y);
                    return da.CompareTo(db);
                });

                int connectionsMade = 0;

                foreach (var other in towers)
                {
                    if (connectionsMade >= maxConnections) break;

                    var path = pathfinder.FindPath(cell, other);
                    
                    if (path != null && path.Count > 0)
                    {
                        var road = SpawnRoadCells(path);
                        tower.Roads.Add(road);
                        connectionsMade++;
                    }
                }

                if (tower.Roads.Count < minConnections)
                {
                    foreach (var other in towers)
                    {
                        if (tower.Roads.Count >= minConnections) break;

                        var path = pathfinder.FindPath(cell, other);
                        if (path != null && path.Count > 0)
                        {
                            var road = SpawnRoadCells(path);
                            tower.Roads.Add(road);
                        }
                    }
                }
            }
        }

        #region Internal

        private List<Cell> SpawnRoadCells(List<Cell> path)
        {
            var road = new List<Cell>();
            
            foreach (var c in path)
            {
                Vector2Int coord = new Vector2Int(c.X, c.Y);
                
                if (_existingRoadCells.TryGetValue(coord, out var existingCell))
                {
                    road.Add(existingCell);
                }
                else
                {
                    var cell = cellFactory.Create(CellType.Road, c.X, c.Y, roadsParent);
                    
                    _existingRoadCells[coord] = cell;
                    
                    road.Add(cell);
                    RoadCells.Add(cell);
                }
            }
            
            return road;
        }

        #endregion

        public IReadOnlyDictionary<Vector2Int, Cell> GetExistingRoadCells()
        {
            return _existingRoadCells;
        }
        
        public bool HasRoadAt(int x, int y)
        {
            return _existingRoadCells.ContainsKey(new Vector2Int(x, y));
        }
        
        public Cell GetRoadCellAt(int x, int y)
        {
            _existingRoadCells.TryGetValue(new Vector2Int(x, y), out var cell);
            return cell;
        }

        public void ClearAllRoads()
        {
            foreach (var cell in _existingRoadCells.Values)
            {
                if (cell != null && cell.gameObject != null)
                {
                    Destroy(cell.gameObject);
                }
            }
            
            _existingRoadCells.Clear();
            
            if (BuildingsGenerator.Instance != null)
            {
                foreach (var towerCell in BuildingsGenerator.Instance.Towers)
                {
                    var building = towerCell.GetComponent<Building>();
                    if (building != null)
                    {
                        building.Roads.Clear();
                    }
                }
            }
            
            RoadCells.Clear();
        }
    }
}
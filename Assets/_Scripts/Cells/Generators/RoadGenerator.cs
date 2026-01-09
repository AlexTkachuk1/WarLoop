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

        public void GenerateRoads(int minConnections = 1, int maxConnections = 3)
        {
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
                        tower.Roads.Add(path);
                        SpawnRoadCells(path);
                        connectionsMade++;
                    }
                }

                if (tower.Roads.Count < minConnections)
                {
                    foreach (var other in towers)
                    {
                        if (tower.Roads.Count >= minConnections) break;

                        var path = pathfinder.FindPath(cell, other);
                        if (path != null && path.Count > 0 && !tower.Roads.Contains(path))
                        {
                            tower.Roads.Add(path);
                            SpawnRoadCells(path);
                        }
                    }
                }
            }
        }

        #region Internal

        private void SpawnRoadCells(List<Cell> path)
        {
            foreach (var c in path)
            {
                cellFactory.Create(CellType.Road, c.X, c.Y, roadsParent);
            }
        }

        #endregion
    }
}

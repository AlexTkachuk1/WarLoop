using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;

public class Pathfinder
{
    private readonly Dictionary<Cell, List<Cell>> _neighborsCache;
    private readonly CellPositionComparer _cellComparer = new CellPositionComparer();

    public Pathfinder(IReadOnlyList<Cell> walkableCells)
    {
        _neighborsCache = BuildNeighborsCache(walkableCells);
    }

    public List<Cell> FindPath(Cell start, Cell goal)
    {
        if (start == null || goal == null)
            return null;
        
        if (_cellComparer.Equals(start, goal))
            return new List<Cell> { start };

        var frontier = new PriorityQueue<Cell>();
        var cameFrom = new Dictionary<Cell, Cell>(_cellComparer);
        var costSoFar = new Dictionary<Cell, float>(_cellComparer);

        frontier.Enqueue(start, 0f);
        cameFrom[start] = start;
        costSoFar[start] = 0f;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current.X == goal.X && current.Y == goal.Y)
                return ReconstructPath(cameFrom, current, start);

            var cacheKey = _neighborsCache.Keys.FirstOrDefault(c => _cellComparer.Equals(c, current));
            if (cacheKey == null || !_neighborsCache.TryGetValue(cacheKey, out var neighbors))
                continue;

            foreach (var neighbor in neighbors)
            {
                float newCost = costSoFar[current] + 1f;

                bool shouldUpdate = true;
                foreach (var visited in costSoFar.Keys)
                {
                    if (_cellComparer.Equals(visited, neighbor))
                    {
                        if (newCost >= costSoFar[visited])
                            shouldUpdate = false;
                        break;
                    }
                }

                if (shouldUpdate)
                {
                    costSoFar[neighbor] = newCost;
                    float priority = newCost + Heuristic(neighbor, goal);
                    frontier.Enqueue(neighbor, priority);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return null;
    }

    private Dictionary<Cell, List<Cell>> BuildNeighborsCache(IReadOnlyList<Cell> walkableCells)
    {
        var cache = new Dictionary<Cell, List<Cell>>();
        
        foreach (var cell in walkableCells)
        {
            var neighbors = FindValidNeighbors(cell, walkableCells);
            cache[cell] = neighbors;
        }
        
        return cache;
    }

    private List<Cell> FindValidNeighbors(Cell cell, IReadOnlyList<Cell> walkableCells)
    {
        var neighbors = new List<Cell>(4);
        
        foreach (var other in walkableCells)
        {
            if (_cellComparer.Equals(other, cell))
                continue;

            if (IsAdjacent(cell, other))
            {
                neighbors.Add(other);
            }
        }
        
        return neighbors;
    }

    private bool IsAdjacent(Cell a, Cell b)
    {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);
        
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    private float Heuristic(Cell a, Cell b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }

    private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current, Cell start)
    {
        var path = new List<Cell>();
        
        while (!_cellComparer.Equals(current, start))
        {
            path.Insert(0, current);
            
            Cell previous = null;
            foreach (var kvp in cameFrom)
            {
                if (_cellComparer.Equals(kvp.Key, current))
                {
                    previous = kvp.Value;
                    break;
                }
            }
            
            if (previous == null) break;
            current = previous;
        }
        
        path.Insert(0, start);
        return path;
    }

    private class PriorityQueue<T>
    {
        private readonly SortedList<(float priority, int id), T> _elements = new SortedList<(float, int), T>(
            Comparer<(float priority, int id)>.Create((a, b) =>
            {
                int cmp = a.priority.CompareTo(b.priority);
                return cmp != 0 ? cmp : a.id.CompareTo(b.id);
            }));
        
        private int _counter;

        public int Count => _elements.Count;

        public void Enqueue(T item, float priority)
        {
            _elements.Add((priority, _counter++), item);
        }

        public T Dequeue()
        {
            if (_elements.Count == 0)
                throw new System.InvalidOperationException("Queue is empty");

            var firstKey = _elements.Keys[0];
            var item = _elements[firstKey];
            _elements.RemoveAt(0);
            return item;
        }
    }

    private class CellPositionComparer : IEqualityComparer<Cell>
    {
        public bool Equals(Cell cellX, Cell cellY)
        {
            return cellX!.X == cellY!.X && cellX.Y == cellY.Y;
        }

        public int GetHashCode(Cell obj)
        {
            if (obj == null) return 0;
            
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + obj.X.GetHashCode();
                hash = hash * 23 + obj.Y.GetHashCode();
                return hash;
            }
        }
    }
}
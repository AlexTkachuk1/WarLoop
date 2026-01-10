using System.Collections.Generic;
using _Scripts.Actors;
using UnityEngine;

namespace _Scripts
{
    public class BuildingsGenerator : Singleton<BuildingsGenerator>
    {
        [Header("References")]
        [SerializeField] private CellFactory buildingsFactory;
        [SerializeField] private Transform buildingsParent;

        private readonly List<Cell> _towers = new();
        public IReadOnlyList<Cell> Towers => _towers;
        
        public void GenerateTowers(int towersCount, int minDistance, int edgeOffset)
        {
            Clear();

            List<Vector2Int> validPositions = GetAllFieldPositions()
                .FindAll(p => !IsEdgePosition(p, edgeOffset));

            if (validPositions.Count == 0)
                return;

            while (_towers.Count < towersCount)
            {
                Vector2Int? candidate = FindValidPosition(validPositions, minDistance);

                if (!candidate.HasValue)
                    break;

                var faction = FactionColor.Black;
                switch (_towers.Count)
                {
                    case 0:
                        faction = FactionColor.Blue;
                        break;
                    case 1:
                        faction = FactionColor.Red;
                        break;
                    case 2:
                        if (Random.value < 0.7f)
                            faction = FactionColor.Purple;
                        else if (Random.value < 0.5f)
                            faction = FactionColor.Gold;
                        break;
                    case 3:
                        if (Random.value < 0.7f)
                            faction = FactionColor.Yellow;
                        else if (Random.value < 0.5f)
                            faction = FactionColor.Gold;
                        break;
                    case 4:
                        if (Random.value < 0.33f)
                            faction = FactionColor.Red;
                        break;
                    case 5:
                        if (Random.value < 0.33f)
                            faction = FactionColor.Red;
                        else if (Random.value < 0.5f)
                            faction = FactionColor.Yellow;
                        else if (Random.value < 0.5f)
                            faction = FactionColor.Purple;
                        break;
                }
                SpawnTower(candidate.Value, faction);
                validPositions.Remove(candidate.Value);
            }
        }

        #region Internal logic

        private void SpawnTower(Vector2Int pos, FactionColor color)
        {
            Cell tower = buildingsFactory.Create(
                CellType.Tower,
                pos.x,
                pos.y,
                buildingsParent,
                color
            );

            _towers.Add(tower);
        }

        private List<Vector2Int> GetAllFieldPositions()
        {
            List<Vector2Int> positions = new();

            for (int x = 0; x < MapGenerator.Instance.Width; x++)
            {
                for (int y = 0; y < MapGenerator.Instance.Height; y++)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }

            return positions;
        }

        private bool IsEdgePosition(Vector2Int pos, int edgeOffset)
        {
            int maxX = MapGenerator.Instance.Width - 1;
            int maxY = MapGenerator.Instance.Height - 1;

            return
                pos.x <= edgeOffset ||
                pos.y <= edgeOffset ||
                pos.x >= maxX - edgeOffset ||
                pos.y >= maxY - edgeOffset;
        }

        private Vector2Int? FindValidPosition(
            List<Vector2Int> candidates,
            int minDistance
        )
        {
            var cells = ListUtils.GetShuffled(candidates);
            
            foreach (var pos in cells)
            {
                bool valid = true;

                foreach (var tower in _towers)
                {
                    int dx = Mathf.Abs(pos.x - tower.X);
                    int dy = Mathf.Abs(pos.y - tower.Y);

                    if (dx < minDistance && dy < minDistance)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                    return pos;
            }

            return null;
        }

        private void Clear()
        {
            foreach (var tower in _towers)
            {
                if (tower)
                    Destroy(tower.gameObject);
            }

            _towers.Clear();
        }

        #endregion
    }
}

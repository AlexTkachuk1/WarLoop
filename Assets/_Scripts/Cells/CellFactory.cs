using UnityEngine;
using System.Collections.Generic;

namespace _Scripts
{
    [CreateAssetMenu(
        fileName = "CellFactory",
        menuName = "Game/Cell Factory"
    )]
    public class CellFactory : ScriptableObject
    {
        [SerializeField] private CellPrefabEntry[] prefabs;

        private Dictionary<CellType, Cell> _cache;

        private void OnEnable()
        {
            _cache = new Dictionary<CellType, Cell>();

            foreach (var entry in prefabs)
            {
                if (!_cache.ContainsKey(entry.type))
                {
                    _cache.Add(entry.type, entry.prefab);
                }
            }
        }

        public Cell Create(CellType type, int x, int y, Transform parent = null)
        {
            if (!_cache.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"Cell prefab for type {type} not found!");
                return null;
            }

            Cell cell = Instantiate(prefab, parent);
            cell.Init(x, y);

            return cell;
        }
    }
}